# Custom Widget for Azure API Management

This folder contains the source code for a custom widget for Azure API Management. The widget is built using React and TypeScript and can be integrated into the Azure API Management portal.

## Project Structure

- **app/**: Contains the main application components.
- **editor/**: Contains the editor components for configuring the widget.
- **hooks.ts**: Contains custom hooks used in the application.
- **main.tsx**: The main entry point for the application.
- **mainEditor.tsx**: The main entry point for the editor.
- **providers.tsx**: Contains context providers for the application.
- **styles/**: Contains the SCSS styles for the application.
- **values.ts**: Contains default values and types used in the application.
- **vite-env.d.ts**: TypeScript declaration file for Vite.
- **index.html**: The main HTML file for the application.
- **editor.html**: The main HTML file for the editor.
- **package.json**: Contains the project dependencies and scripts.
- **tsconfig.json**: TypeScript configuration file.
- **tsconfig.node.json**: TypeScript configuration file for Node.js.
- **vite.config.ts**: Vite configuration file is using env.process to get the values from the environment variables so make sure you have your own `.env.development` and `.env.production` files.

## Running the Project Locally

To run the project locally, follow these steps:

1. **Install Dependencies**: Make sure you have Node.js and npm installed. Then, install the project dependencies by running:

    ```sh
    npm install
    ```

2. **Start the Development Server**: Start the Vite development server by running:

    ```sh
    npm start
    ```

    This will start the development server and open the application in your default web browser.

3. **Open Developer Portal as Admin**: You want to append `?MS_APIM_CW_localhost_port=<port-number>` to the end of the URL to run the developer portal in the context of the custom widget. The port number should match the port number that Vite is running on.

    For example, if Vite is running on port 3000, you would append `?MS_APIM_CW_localhost_port=3000` to the developer portal URL.

    > Note: You may need to log in as an admin to access the developer portal in edit mode.

## Deploying the Widget

To deploy the widget to Azure API Management, follow these steps:

1. **Upload the Widget**: Upload the widget to Azure API Management by following the steps outlined in the [Azure API Management documentation](https://learn.microsoft.com/en-us/azure/api-management/developer-portal-extend-custom-functionality#deploy-the-custom-widget-to-the-developer-portal).

*If for some reason when you deploy this following the instructions in the docs and it doesn't prompt you for your login and it's failing to deploy, you can override the token to use one you know will work by setting the `tokenOverride` propertied on the serviceInformation object.*

##### deploy.js

```js
const serviceInformation = {
"resourceId": "<full-apim-resource_id>",
"managementApiEndpoint": "https://management.azure.com",
"apiVersion": "2022-08-01",
"tokenOverride": "<your-token>"
}
```

## Additional Information

For more information on building custom widgets for Azure API Management, refer to the [Azure API Management documentation](https://learn.microsoft.com/en-us/azure/api-management/developer-portal-extend-custom-functionality#create-and-upload-custom-widget).
