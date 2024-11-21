namespace AuthHelper
{
    public class JwtOptions
    {
        public string Key { get; set; } = string.Empty;
        public int ExpiratesMinutes { get; set; }
    }
}