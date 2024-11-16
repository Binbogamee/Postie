namespace AccountService
{
    public class AccountManager
    {
        private static readonly Lazy<AccountManager> _instance = new Lazy<AccountManager>(() => new AccountManager());

        public static AccountManager Instance => _instance.Value;

        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public void VerifyPassword(string password, string passwordHash)
        {
            if (!BCrypt.Net.BCrypt.Verify(password, passwordHash))
            {
                throw new Exception("Wrong email or password");
            }
        }
    }
}
