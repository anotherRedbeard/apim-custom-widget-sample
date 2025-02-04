using Azure.Core;
using Azure.Identity;
using Microsoft.Extensions.Logging;

namespace APIMCustomerWidget.Services
{
    public class TokenService
    {
        private readonly TokenCredential _credential;
        private readonly ILogger<TokenService> _logger;

        public TokenService(ILogger<TokenService> logger)
        {
            _credential = new DefaultAzureCredential();
            _logger = logger;
        }

        public TokenCredential Credential => _credential;

        public async Task<string> GetGraphApiTokenAsync()
        {
            var tokenRequestContext = new TokenRequestContext(new[] { "https://graph.microsoft.com/.default" });
            var accessToken = await _credential.GetTokenAsync(tokenRequestContext, default);
            _logger.LogInformation("Obtained access token for Graph API.");
            return accessToken.Token;
        }

    }
}