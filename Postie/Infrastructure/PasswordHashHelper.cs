namespace Postie.Infrastructure
{
    public class PasswordHashHelper
    {
        private static readonly Lazy<PasswordHashHelper> _instance = new Lazy<PasswordHashHelper>(() => new PasswordHashHelper());

        public static PasswordHashHelper Instance => _instance.Value;

        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public bool VerifyPassword(string password, string passwordHash)
        {
            if (!BCrypt.Net.BCrypt.Verify(password, passwordHash))
            {
                return false;
            }

            return true;
        }
    }
}
