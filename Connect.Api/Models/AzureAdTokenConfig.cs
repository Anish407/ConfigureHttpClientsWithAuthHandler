namespace Connect.Api.Models
{
    public class AzureAdTokenConfig
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string TenantId { get; set; }
        public string Scope { get; set; }
    }
}
