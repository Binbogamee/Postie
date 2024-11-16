namespace AccountService.Jwt
{
    public class JwtOptions
    {
        public string Key { get; set; } = string.Empty;
        public int ExpiratesHours { get; set; }
    }
}