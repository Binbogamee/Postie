using Microsoft.AspNetCore.Mvc;
using Postie.Models;

namespace Postie.Infrastructure
{
    public static class ControllerResultMapper
    {
        public static ObjectResult ResultMapper(ErrorType type, string error)
        {
            switch (type)
            {
                case ErrorType.NotFound:
                    return new ObjectResult(error) { StatusCode = StatusCodes.Status404NotFound };
                case ErrorType.AccessDenied:
                    return new ObjectResult(error) { StatusCode = StatusCodes.Status403Forbidden };
                case ErrorType.ValidationError:
                    return new ObjectResult(error) { StatusCode = StatusCodes.Status422UnprocessableEntity };
                default:
                    return new ObjectResult(error) { StatusCode = StatusCodes.Status500InternalServerError };
            }

        }
    }
}
