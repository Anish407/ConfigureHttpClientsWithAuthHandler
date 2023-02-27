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
            // get the access token from azure ad
            var accessToken = await _azureAdTokenHandler.GetAccessTokenFromAzureAd();

            // set the access token in the header
            request.Headers.Add("Authorization", $"bearer {accessToken}");

            // call the api/ next handler (weather forecast)
            var response = await base.SendAsync(request, cancellationToken);

            // return response to WeatherService
            return response;    
        }
    }
}
