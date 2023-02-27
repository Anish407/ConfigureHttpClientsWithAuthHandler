using Azure.Core;
using Connect.Api.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using static Connect.Api.Common.Constants.Constants;

namespace Connect.Api.HttpClients.Handler
{
    public class AzureAdTokenHandler : IAzureAdTokenHandler
    {
        private readonly IMemoryCache _memoryCache;
        private readonly AzureAdTokenConfig _adTokenConfig;

        public AzureAdTokenHandler(IMemoryCache memoryCache, IOptions<AzureAdTokenConfig> options)
        {
            _memoryCache = memoryCache;
            _adTokenConfig = options.Value;
        }


        public async Task<string> GetAccessTokenFromAzureAd()
        {
            SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1);
            var tokenModel = new TokenModel();
            tokenModel = _memoryCache.Get<TokenModel>(AzureAdMemoryCacheKey);
            if (tokenModel != null)
            {
                return tokenModel.AccessToken;
            }

            try
            {
                semaphoreSlim.Wait();
                tokenModel = _memoryCache.Get<TokenModel>(AzureAdMemoryCacheKey);
                if (tokenModel != null)
                {
                    return tokenModel.AccessToken;
                }


                string[] scopes = new string[] { $"{_adTokenConfig.ClientId}/.default" };

                IConfidentialClientApplication app = ConfidentialClientApplicationBuilder
                    .Create(_adTokenConfig.ClientId)
                    .WithClientSecret(_adTokenConfig.ClientSecret)
                    .WithAuthority($"https://login.microsoftonline.com/{_adTokenConfig.TenantId}/")
                    .WithTenantId(_adTokenConfig.TenantId)
                    .Build();

                var result = await app.AcquireTokenForClient(scopes).ExecuteAsync();
                _memoryCache.Set(AzureAdMemoryCacheKey, new TokenModel
                {
                    AccessToken = result.AccessToken,
                    Expiry = result.ExpiresOn
                });

                return result.AccessToken;
            }
            catch (Exception ex)
            {

            }
            finally
            {
                semaphoreSlim.Release();
            }

            return string.Empty;
        }
    }



    public class TokenModel
    {
        public string AccessToken { get; set; }
        public DateTimeOffset Expiry { get; set; }
    }
}
