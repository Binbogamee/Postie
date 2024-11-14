namespace Postie.Models
{
    public class Account
    {
        public Account()
        {
            Id = Guid.Empty;
        }

        public Account(string username, string email, string password, Guid? id = null)
        {
            Id = id ?? Guid.NewGuid();
            Username = username;
            Email = email;
            PasswordHash = password;
        }

        public Guid Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
    }
}
