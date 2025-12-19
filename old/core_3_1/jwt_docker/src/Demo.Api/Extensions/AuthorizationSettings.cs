namespace Demo.Api.Extensions
{
    public class AuthorizationSettings
    {
        public string SecurityKey { get; set; }
        public int ExpirationMinutes { get; set; }
        public string Sender { get; set; }
        public string ValidOn { get; set; }
    }
}