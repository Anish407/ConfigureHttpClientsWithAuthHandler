namespace Connect.Api.HttpClients.Handler
{
    public interface IAzureAdTokenHandler
    {
        Task<string> GetAccessTokenFromAzureAd();
    }
}