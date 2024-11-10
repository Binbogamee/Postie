namespace Postie.Configurations
{
    public static class ServicesRouts
    {
        public enum Services 
        {
            Post,
            Logging,
            Account
        }
        public static readonly Dictionary<Services, string> Routs = new Dictionary<Services, string>()
        {
            { Services.Post, "https://localhost:44360/Post" }
        };
    }
}
