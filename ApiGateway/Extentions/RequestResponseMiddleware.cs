﻿using System.Net;
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

            await CompleteHeader(context.Request);

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

        private async Task CompleteHeader(HttpRequest request)
        {

            var authstring = _jwtTokenManager.GetRequestToken(request.Headers.Authorization);

            if (String.IsNullOrEmpty(authstring))
            {
                return;
            }

            var requesterId = _jwtTokenManager.GetJwtClaimAccountId(authstring);
            if (requesterId == null)
            {
                return;
            }

            request.Headers[Shared.CustomHeaders.UserId] = requesterId.Value;
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
