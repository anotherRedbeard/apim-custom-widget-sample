#!/bin/bash
# This script creates a resource group, storage account, and function app in Azure. It also has an option to clean up 
# and delete the resource group. You can also just deploy the function app by providing the required arguments.

# Function to display usage
usage() {
    echo "Usage: $0 [-c] [-d] <resource-group-name> <function-app-name> <storage-account-name> <location>"
    echo "  -c    Clean up and delete the resource group"
    echo "  -d    Deploy the function app"
    exit 1
}

# Check if at least 4 arguments are provided
if [ "$#" -lt 4 ]; then
    usage
    exit 1
fi

# Parse the clean up and deploy only flags
CLEANUP=false
DEPLOY_ONLY=false
while getopts "cd" opt; do
    case $opt in
        c)
            CLEANUP=true
            ;;
        d)
            DEPLOY_ONLY=true
            ;;
        *)
            usage
            return 1
            ;;
    esac
done
shift $((OPTIND -1))

# Assign variables
RESOURCE_GROUP=$1
FUNCTION_APP=$2
STORAGE_ACCOUNT=$3
LOCATION=$4

# Clean up resource group if the flag is set
if [ "$CLEANUP" = true ]; then
    echo "Deleting Resource group $RESOURCE_GROUP."
    az group delete --name $RESOURCE_GROUP --yes --no-wait
    echo "Resource group $RESOURCE_GROUP deletion initiated."
    exit 0
fi

# Deploy only if the flag is set
if [ "$DEPLOY_ONLY" = true ]; then
    echo "Deploying Function app $FUNCTION_APP."
    func azure functionapp publish $FUNCTION_APP
    echo "Function app $FUNCTION_APP deployment completed."
    exit 0
fi

# Create resource group
az group create --name $RESOURCE_GROUP --location $LOCATION

# Create storage account
az storage account create --name $STORAGE_ACCOUNT --location $LOCATION --resource-group $RESOURCE_GROUP --sku Standard_LRS

# Create function app
az functionapp create --resource-group $RESOURCE_GROUP --consumption-plan-location $LOCATION --runtime dotnet --functions-version 4 --name $FUNCTION_APP --storage-account $STORAGE_ACCOUNT --assign-identity

# Deploy function app (assuming you have a local package to deploy)
# Replace <path-to-package> with the actual path to your function app package
func azure functionapp publish $FUNCTION_APP

echo "Script execution completed."