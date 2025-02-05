namespace Postie.Dtos
{
    public record NewAccountRequest
    (
        string Username,
        string Email,
        string Password
    );

    public record AccountDto
    (
        Guid Id,
        string Username,
        string Email
    );

    public record PasswordChangeRequest
    (
        string OldPassword,
        string NewPassword
    );

    public record LoginRequest
    (
        string Email,
        string Password
    );
}
