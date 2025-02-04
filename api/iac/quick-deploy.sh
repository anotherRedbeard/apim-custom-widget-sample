#!/bin/bash

# Function to display usage
usage() {
    echo "Usage: $0 [-c] <resource-group-name> <function-app-name> <storage-account-name> <location>"
    echo "  -c    Clean up and delete the resource group"
    return 1
}

# Check if at least 4 arguments are provided
if [ "$#" -lt 4 ]; then
    usage
    return 1
fi

# Parse the clean up flag
CLEANUP=false
if [ "$1" == "-c" ]; then
    CLEANUP=true
    shift
fi

# Assign variables
RESOURCE_GROUP=$1
FUNCTION_APP=$2
STORAGE_ACCOUNT=$3
LOCATION=$4

# Clean up resource group if the flag is set
if [ "$CLEANUP" = true ]; then
    az group delete --name $RESOURCE_GROUP --yes --no-wait
    echo "Resource group $RESOURCE_GROUP deletion initiated."
    return 0
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