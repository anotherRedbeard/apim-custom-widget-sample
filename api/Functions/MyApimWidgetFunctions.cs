using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using APIMCustomerWidget.Models;
using APIMCustomerWidget.Services;
using System.Security.Principal;

namespace APIMCustomerWidget.Functions
{
    public class MyApimWidgetFunctions
    {
        private readonly ILogger<MyApimWidgetFunctions> _logger;
        private readonly AuthenticationService _authService;
        private readonly GraphService _graphService;

        public MyApimWidgetFunctions(ILogger<MyApimWidgetFunctions> logger, 
            AuthenticationService authService,
            GraphService graphService)
        {
            _logger = logger;
            _authService = authService;
            _graphService = graphService;
        }

        /// <summary>
        /// GetAppRegistration function
        /// Handles HTTP requests for getting app registrations.
        /// </summary>
        /// <param name="req">The HTTP request data.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a collection of app registrations.</returns>
        [Function("GetAppRegistration")]
        public async Task<HttpResponseData> GetAppRegistration(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", "options")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            //Authenticate user
            var (user, authStatus) = await _authService.AuthenticateUser(req);
            if (authStatus != AuthStatus.Valid || user == null)
            {
                var response = req.CreateResponse(System.Net.HttpStatusCode.Unauthorized);
                response.Headers.Add("Content-Type", "application/json");
                await response.WriteStringAsync("Unauthorized");
                return response;
            }

            // Call Graph API to get the app registrations
            var appRegistrations = await _graphService.GetAppRegistrationsAsync();
            var jsonResponse = req.CreateResponse(System.Net.HttpStatusCode.OK);
            jsonResponse.Headers.Add("Content-Type", "application/json");
            await jsonResponse.WriteStringAsync(appRegistrations != null ? JsonSerializer.Serialize(appRegistrations) : "No app registrations found");

            return jsonResponse;
        }

        /// <summary>
        /// PostAppRegistration function
        /// Handles HTTP requests for creating app registrations.
        /// </summary>
        /// <param name="req">The HTTP request data that contains the body of the request which should contain a JSON 
        /// representation of an app registration/// </param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the newly created app registration.</returns>

        [Function("PostAppRegistration")]
        public async Task<HttpResponseData> PostAppRegistration(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", "options")] HttpRequestData req)
        {
            _logger.LogInformation("Processed PostAppRegistration request.");
            //confirm request body is not empty
            if (req.Body == null)
            {
                var response = req.CreateResponse(System.Net.HttpStatusCode.BadRequest);
                response.Headers.Add("Content-Type", "application/json");
                await response.WriteStringAsync("Request body is empty");
                return response;
            }

            //Authenticate user
            var (user, authStatus) = await _authService.AuthenticateUser(req);
            if (authStatus != AuthStatus.Valid || user == null)
            {
                var response = req.CreateResponse(System.Net.HttpStatusCode.Unauthorized);
                response.Headers.Add("Content-Type", "application/json");
                await response.WriteStringAsync("Unauthorized");
                return response;
            }

            try 
            {
                // Call Graph API to create an app registration using the request body
                var requestBody = await req.ReadAsStringAsync();
                _logger.LogInformation($"Request body: {requestBody}");
                if (string.IsNullOrEmpty(requestBody))
                {
                    var response = req.CreateResponse(System.Net.HttpStatusCode.BadRequest);
                    response.Headers.Add("Content-Type", "application/json");
                    await response.WriteStringAsync("Request body is empty or null");
                    return response;
                }

                var appRegistration = await _graphService.PostAppRegistrationAsync(requestBody);
                var jsonResponse = req.CreateResponse(System.Net.HttpStatusCode.OK);
                jsonResponse.Headers.Add("Content-Type", "application/json");
                await jsonResponse.WriteStringAsync(appRegistration != null ? JsonSerializer.Serialize(appRegistration) : "Error creating App Registration");
                return jsonResponse;
            }
            catch (ApplicationException ex)
            {
                var errorResponse = req.CreateResponse(System.Net.HttpStatusCode.BadRequest);
                errorResponse.Headers.Add("Content-Type", "application/json");
                await errorResponse.WriteStringAsync(ex.Message);
                return errorResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError($"An unexpected error occurred: {ex.Message}");
                var errorResponse = req.CreateResponse(System.Net.HttpStatusCode.InternalServerError);
                errorResponse.Headers.Add("Content-Type", "application/json");
                await errorResponse.WriteStringAsync("An unexpected error occurred while processing your request.");
                return errorResponse;
            }
        }
    }
}