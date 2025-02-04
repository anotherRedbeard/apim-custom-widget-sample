using Microsoft.Extensions.Logging;
using System.Text.Json;
using APIMCustomerWidget.Models;
using Microsoft.Azure.Functions.Worker.Http;

namespace APIMCustomerWidget.Services
{
    public class AuthenticationService
    {
        private readonly ILogger<AuthenticationService> _logger;
        private readonly HttpClient _httpClient;

        public AuthenticationService(ILogger<AuthenticationService> logger, HttpClient httpClient)
        {
            _logger = logger;
            _httpClient = httpClient;
        }

        public async Task<(ApimUser?, AuthStatus)> AuthenticateUser(HttpRequestData req)
        {
            if (!req.Headers.TryGetValues("x-apim-management-url", out var managementApiUrlValue) ||
                !req.Headers.TryGetValues("x-apim-api-version", out var apiVersionValue) ||
                !req.Headers.TryGetValues("authorization", out var userTokenValues) ||
                !req.Headers.TryGetValues("x-ms-user-id", out var userIdValues))
            {
                return (null, AuthStatus.MissingHeaders);
            }

            var userToken = userTokenValues != null ? string.Join(",", userTokenValues) : "No token";
            var userId = userIdValues != null ? string.Join(",", userIdValues) : "No user ID";
            var managementApiUrl = managementApiUrlValue != null ? string.Join(",", managementApiUrlValue) : "No management API URL";
            var apiVersion = apiVersionValue != null ? string.Join(",", apiVersionValue) : "No API version";

            if (string.IsNullOrEmpty(userToken))
            {
                return (null, AuthStatus.NoToken);
            }

            try
            {
                var user = await FetchUserDetailsFromApim(userId, userToken, managementApiUrl, apiVersion);
                return (user, AuthStatus.Valid);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during user authentication");
                return (null, AuthStatus.Error);
            }
        }

        private async Task<ApimUser?> FetchUserDetailsFromApim(string userId, string token, string managementApiUrl, string apiVersion)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{managementApiUrl}/users/{userId}?api-version={apiVersion}");
            request.Headers.Add("Authorization", token);

            var response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                // Log raw response
                _logger.LogInformation($"User Response: {content}");

                // Deserialize the response into an ApimUser object
                var result = JsonDocument.Parse(content).RootElement;
                var user = new ApimUser
                {
                    Id = result.GetProperty("name").GetString() ?? string.Empty,
                    Email = result.GetProperty("properties").GetProperty("email").GetString() ?? string.Empty,
                    FirstName = result.GetProperty("properties").GetProperty("firstName").GetString() ?? string.Empty,
                    LastName = result.GetProperty("properties").GetProperty("lastName").GetString() ?? string.Empty,
                    State = result.GetProperty("properties").GetProperty("state").GetString() ?? string.Empty,
                };

                // Get user groups
                var groups = await FetchUserGroupsFromApim(userId, token, managementApiUrl, apiVersion);
                user.Groups = groups;

                return user;
            }

            return null;
        }

        private async Task<List<string>> FetchUserGroupsFromApim(string userId, string token, string managementApiUrl, string apiVersion)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{managementApiUrl}/users/{userId}/groups?api-version={apiVersion}");
            request.Headers.Add("Authorization", token);

            var response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                // Log raw response
                _logger.LogInformation($"Group Response: {content}");

                // Deserialize the response into a list of group names
                var result = JsonDocument.Parse(content).RootElement;
                var groups = new List<string>();
                foreach (var group in result.GetProperty("value").EnumerateArray())
                {
                    groups.Add(group.GetProperty("name").GetString() ?? string.Empty);
                }
                return groups;
            }

            return new List<string>();
        }
    }
}