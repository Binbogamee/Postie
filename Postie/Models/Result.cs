namespace Postie.Models
{
    public class Result<T>
    {
        public bool IsSuccess { get; set; }
        public T Value { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
        public ErrorType Error { get; set; }

        public static Result<T> Success(T value) => new Result<T> { IsSuccess = true, Value = value };
        public static Result<T> Failure(ErrorType error, string message = "") =>
            new Result<T> { IsSuccess = false, ErrorMessage = message, Error = error };
    }

    public enum ErrorType
    {
        NotFound,
        AccessDenied,
        ValidationError
    }
}
