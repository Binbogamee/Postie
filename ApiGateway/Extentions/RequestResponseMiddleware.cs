using System.Net;
using System.Text;
using Newtonsoft.Json;
using Shared.KafkaLogging;
using LogLevel = NLog.LogLevel;

namespace ApiGateway.Extentions
{
    public class RequestResponseMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly JwtTokenManager _jwtTokenManager;

        public RequestResponseMiddleware(RequestDelegate next)
        {
            _jwtTokenManager = JwtTokenManager.Instance;
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var _loggingProducerService = context.RequestServices.GetRequiredService<ILoggingProducerService>();

            context.Request.EnableBuffering();

            await ModifyRequest(context.Request);

            var builder = new StringBuilder();
            builder.Append(await FormatRequest(context.Request));

            var originalBodyStream = context.Response.Body;
            using var temporaryBodyStream = new MemoryStream();
            context.Response.Body = temporaryBodyStream;

            await _next(context);

            builder.Append(await FormatResponse(context.Response));

            var status = context.Response.StatusCode;
            if (status >= 500)
            {
                context.Response.ContentLength = 0;
            }

            if (status < 300)
            {
                _loggingProducerService.SendLogMessage(LogLevel.Info, builder.ToString());
            }
            else if (status < 500)
            {
                _loggingProducerService.SendLogMessage(LogLevel.Warn, builder.ToString());
            }
            else if (status < 600)
            {
                _loggingProducerService.SendLogMessage(LogLevel.Error, builder.ToString());
            }

            await temporaryBodyStream.CopyToAsync(originalBodyStream);
            context.Response.Body = originalBodyStream;
        }

        private async Task ModifyRequest(HttpRequest request)
        {

            var authstring = _jwtTokenManager.GetRequestToken(request.Headers.Authorization);

            if (String.IsNullOrEmpty(authstring))
            {
                return;
            }

            if (request.Method != HttpMethods.Put &&
                request.Method != HttpMethods.Delete &&
                !(request.Path == "/api/Post" && request.Method == HttpMethods.Post))
            {
                return;
            }

            var requesterId = _jwtTokenManager.GetJwtClaimAccountId(authstring);
            if (requesterId == null)
            {
                return;
            }

            request.Body.Seek(0, SeekOrigin.Begin);
            var body = await new StreamReader(request.Body).ReadToEndAsync();
            request.Body.Seek(0, SeekOrigin.Begin);

            try
            {
                var json = JsonConvert.DeserializeObject<Dictionary<string, object>>(body);
                if (json == null)
                {
                    json = new Dictionary<string, object>();
                    request.ContentType = "application/json";
                }
                json.Add(requesterId.Type, requesterId.Value);

                var modifiedBody = JsonConvert.SerializeObject(json, Formatting.Indented);
                var bytes = Encoding.UTF8.GetBytes(modifiedBody);
                request.Body = new MemoryStream(bytes);
                request.ContentLength = bytes.Length;
            }
            catch { }
        }

        private async Task<string> FormatRequest(HttpRequest request)
        {
            var formattedRequest = $"{request.Method} | {request.Path} | ";

            return formattedRequest;
        }

        private async Task<string> FormatResponse(HttpResponse response)
        {
            string text = string.Empty;
            response.Body.Seek(0, SeekOrigin.Begin);

            if (response.StatusCode >= 400)
            {
                text = await new StreamReader(response.Body).ReadToEndAsync();
                response.Body.Seek(0, SeekOrigin.Begin);
            }

            var statusName = Enum.GetName(typeof(HttpStatusCode), response.StatusCode);
            return $"{response.StatusCode} - {statusName} | {text}";
        }
    }
}
