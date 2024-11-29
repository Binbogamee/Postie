namespace Postie.Dtos
{
    public record NewAccountRequest
    (
        string Username,
        string Email,
        string Password
    );

    public record AccountResponse
    (
        Guid Id,
        string Username,
        string Email
    );

    public record AccountRequest
    (
        Guid RequesterId,
        string Username,
        string Email
    );

    public record PasswordChangeRequest
    (
        Guid RequesterId,
        string OldPassword,
        string NewPassword
    );

    public record LoginRequest
    (
        string Email,
        string Password
    );
}
