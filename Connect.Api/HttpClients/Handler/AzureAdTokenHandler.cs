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
            // allows only 1 thread at a time
            SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1);
            // check if token exists in the cache
            var tokenModel = new TokenModel();
            tokenModel = _memoryCache.Get<TokenModel>(AzureAdMemoryCacheKey);
            if (tokenModel != null)
            {
                // return token from cache
                return tokenModel.AccessToken;
            }

            try
            {
                // only 1 thread can enter this block at once
                semaphoreSlim.Wait();

                // check cache again
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

                // call azure ad to get the token
                var result = await app.AcquireTokenForClient(scopes).ExecuteAsync();

                // set the token in the cache
                _memoryCache.Set(AzureAdMemoryCacheKey, new TokenModel
                {
                    AccessToken = result.AccessToken,
                    Expiry = result.ExpiresOn
                });

                // return the access token
                return result.AccessToken;
            }
            catch (Exception ex)
            {

            }
            finally
            {
                // release the lock. next thread can enter now
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
