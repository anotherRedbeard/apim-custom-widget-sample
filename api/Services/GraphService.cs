using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

namespace APIMCustomerWidget.Services
{
    public class GraphService
    {
        private readonly HttpClient _httpClient;
        private readonly TokenService _tokenService;
        private readonly ILogger<GraphService> _logger;
        private readonly GraphServiceClient _graphServiceClient;

        public GraphService(HttpClient httpClient, 
            ILogger<GraphService> logger,
            TokenService tokenService)
        {
            _httpClient = httpClient;
            _logger = logger;
            _tokenService = tokenService;
            _graphServiceClient = new GraphServiceClient(_tokenService.Credential);
        }

        public async Task<Application?> PostAppRegistrationAsync(string requestBody)
        {
            try
            {

                // configure JsonSerializer to handle camelCase JSON to PascalCase object mapping
                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    PropertyNameCaseInsensitive = true
                };
                //convert the request body JSON to the Application object
                Application? app = JsonSerializer.Deserialize<Application>(requestBody,options);
                if (app == null || string.IsNullOrEmpty(app.DisplayName))
                {
                    _logger.LogError("Deserialization of requestBody to Application object failed.");
                    return null;
                }
                var result = await _graphServiceClient.Applications.PostAsync(app);
                return result;
            }
            catch (ServiceException ex)
            {
                _logger.LogError($"Graph API call failed with status code {ex.ResponseStatusCode} and message: {ex.Message}");
                throw new ApplicationException($"Failed to post app registration: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An unexpected error occurred: {ex.Message}");
                throw new ApplicationException("An unexpected error occurred while posting app registration.", ex);
            }
        }

        public async Task<string> PostAppRegistrationRawAsync(string requestBody)
        {
            var graphApiUrl = "https://graph.microsoft.com/v1.0/applications";
            var graphRequest = new HttpRequestMessage(HttpMethod.Post, graphApiUrl);

            // Get token for Graph API
            var token = await _tokenService.GetGraphApiTokenAsync();

            graphRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            graphRequest.Content = new StringContent(requestBody);
            graphRequest.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            _logger.LogInformation("Calling Graph API to create app registration.");
            var graphResponse = await _httpClient.SendAsync(graphRequest);

            if (!graphResponse.IsSuccessStatusCode)
            {
                var errorContent = await graphResponse.Content.ReadAsStringAsync();
                _logger.LogError($"Graph API call failed with status code {graphResponse.StatusCode} and message: {errorContent}");
            }

            return await graphResponse.Content.ReadAsStringAsync();
        }


        public async Task<ApplicationCollectionResponse?> GetAppRegistrationsAsync()
        {
            // get all app registrations and order them by display name
            var appRegistrations = await _graphServiceClient.Applications.GetAsync((c) =>
            {
                c.QueryParameters.Count = true;
            });

            return appRegistrations;
        }
        public async Task<HttpResponseMessage> GetAppRegistrationsRawAsync()
        {
            // get all app registrations and order them by display name
            var graphApiUrl = "https://graph.microsoft.com/v1.0/applications?$count=true";
            var graphRequest = new HttpRequestMessage(HttpMethod.Get, graphApiUrl);

            // Get token for Graph API
            var token = await _tokenService.GetGraphApiTokenAsync();

            graphRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            _logger.LogInformation("Calling Graph API to get app registrations.");
            var graphResponse = await _httpClient.SendAsync(graphRequest);

            if (!graphResponse.IsSuccessStatusCode)
            {
                var errorContent = await graphResponse.Content.ReadAsStringAsync();
                _logger.LogError($"Graph API call failed with status code {graphResponse.StatusCode} and message: {errorContent}");
            }

            return graphResponse;
        }
    }
}