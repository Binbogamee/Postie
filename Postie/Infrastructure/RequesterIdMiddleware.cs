namespace Postie.Infrastructure
{
    public class RequesterIdMiddleware
    {
        public const string UserIdName = "UserId";

        private readonly RequestDelegate _next;

        public RequesterIdMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Headers.TryGetValue(Shared.CustomHeaders.UserId, out var userId))
            {
                if (Guid.TryParse(userId, out var guid))
                {
                    context.Items[UserIdName] = guid;
                }
                else
                {
                    context.Items[UserIdName] = Guid.Empty;
                }
            }

            await _next(context);
        }
    }
}
