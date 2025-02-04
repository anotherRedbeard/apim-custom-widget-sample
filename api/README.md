# API - Azure Functions for APIM Customer Widget

This folder contains the Azure Functions implementation for the APIM Customer Widget. The main functions in this project are `GetAppRegistration` and `PostAppRegistration`, which handle HTTP requests to fetch user details and create app registrations in the Azure API Management (APIM) service.

## Project Structure

- **api.csproj**: The project file for the Azure Functions project.
- **Program.cs**: The entry point for the Azure Functions application.
- **Functions/MyApimWidgetFunctions.cs**: Contains the implementation of all the functions required for the custom widget.
- **host.json**: Configuration file for the Azure Functions host.
- **local.settings.json**: Local settings for the Azure Functions project (not included in source control).
- **Models/**: Contains the model classes used in the project.
- **Services/**: Contains the service classes used in the project.

## Function Overview

### GetAppRegistration

The `GetAppRegistration` function is an HTTP-triggered function that processes requests to fetch user details and their associated groups from the APIM service.

#### Request Headers

The function expects the following headers in the request:

- `x-apim-management-url`: The URL of the APIM management endpoint.
- `x-apim-api-version`: The API version to use for the APIM requests.
- `authorization`: The bearer token for authenticating with the APIM service.
- `x-ms-user-id`: The user ID of the user to fetch details for.

#### Response

The function returns a JSON response containing the user details and their associated groups. If any required headers are missing or if there is an error during authentication, the function returns an appropriate error response.

### PostAppRegistration

The `PostAppRegistration` function is an HTTP-triggered function that processes requests to create a new app registration in the Azure AD.

#### Request Body

The function expects the following JSON body in the request:

```json
{
  "displayName": "Display name",
  "description": "Description",
  "spa": {
    "redirectUris": ["https://example.com"]
  }
}
```

#### Response

The function returns a JSON response containing the newly created app registration details. If there is an error during the creation process, the function returns an appropriate error response.

### Helper Methods

#### CreateCorsResponse

Creates a CORS response to handle preflight requests and set the necessary CORS headers.

#### AuthenticateUser

Authenticates the user by fetching their details from the APIM service using the provided token. Returns the user details and an authentication status.

### Services

#### AuthenticationService

The `AuthenticationService` class handles the authentication of users by validating the provided token and fetching user details from the APIM service.

#### GraphService

The `GraphService` class handles interactions with the Microsoft Graph API, including fetching app registrations and creating new app registrations.

#### TokenService

The `TokenService` class handles the acquisition of tokens using `DefaultAzureCredential` and provides the token for use in the `GraphService`.

### Running the Project

To run the project locally, use the following command:

```sh
func start
```

Make sure to provide the required environment variables in the `local.settings.json` file.

### Deployment

To deploy the project to Azure, use the following command:

```sh
func azure functionapp publish <function-app-name>
```

Replace <function-app-name> with the name of your Azure Functions app.

### Additional Information

For more information on Azure Functions, refer to the [Azure Functions documentation](https://learn.microsoft.com/en-us/azure/azure-functions/).