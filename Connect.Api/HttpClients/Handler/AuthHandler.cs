namespace Connect.Api.HttpClients.Handler
{
    public class AuthHandler: DelegatingHandler
    {
        private readonly IAzureAdTokenHandler _azureAdTokenHandler;

        public AuthHandler(IAzureAdTokenHandler azureAdTokenHandler)
        {
            _azureAdTokenHandler = azureAdTokenHandler;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, 
            CancellationToken cancellationToken)
        {
            var accessToken = await _azureAdTokenHandler.GetAccessTokenFromAzureAd();

            request.Headers.Add("Authorization", $"bearer {accessToken}");

            var response = await base.SendAsync(request, cancellationToken);

            return response;    
        }
    }
}
