# ToDoWebApi Project
- [Simple UI Wrapper in Azure](https://pichetodoitems.azurewebsites.net)
- [Swagger UI in Azure](https://pichetodoitems.azurewebsites.net/swagger/index.html)

API Availability Toggling without Restart or Deployment

## Overview

This project is an ASP.NET Core API for managing to-do items. It supports CRUD operations and is designed to be deployed using Azure Web App with a CI/CD pipeline integrated with GitHub.

## Features

- Get all to-do items.
- Get a to-do item by ID.
- Create a new to-do item.
- Update an existing to-do item.
- Delete a to-do item.
- Enable/Disable APIs without re-deployment or restart of the application.

## Prerequisites

- [.NET 6.0 SDK](https://dotnet.microsoft.com/download/dotnet/6.0)
- [Azure CLI](https://docs.microsoft.com/en-us/cli/azure/install-azure-cli)
- [Git](https://git-scm.com/downloads)
- [An Azure Account](https://azure.microsoft.com/en-us/free/)

## Local Development

### Clone the repository

git clone https://github.com/helmutsreinis/ToDoWebApi.git

### Restore dependencies

Navigate to the project folder and run:

dotnet restore

### Run the application

dotnet run

Your API should now be running on `http://localhost` on port 7226 or 5000, depending on your local setup.

## Running Tests

To run unit tests, navigate to the test project folder and run:

dotnet test

## Deployment to Azure

Ensure that you have provisioned a Web App, a free tier plan will suffice for this.

The project is configured to be deployed to Azure using a CI/CD pipeline through GitHub Actions.

### Steps to set up CI/CD with Azure Web App and GitHub

1. Create a Web App on Azure Portal.
2. In the Deployment Center, choose GitHub as the source for your application.
3. Authorize Azure to access your GitHub repository.
4. Choose the branch from which Azure will deploy (usually `master` or `main`).
5. Save and finish the setup.

Now, any push to the specified branch will trigger a build and deployment process.

### Rollback Mechanism

In case a deployment fails, Azure Web App supports rollback mechanisms that can be configured to revert the application to a previous state.

# ((Important)) Enable Tests and Feature Toggling
## Modify workflow file in your Github Actions 

After you have created CI/CD Pipeline, modify the workflow file to include an additional step right after the step '''Build with dotnet'''

      - name: Run Unit Tests
        run: dotnet test --configuration Release

## Toggling Features on the fly without redeployment

### appsettings.json 
The file will contain a list of features and the states. After you have deployed your application to productions, you will need access to the hosting environment to access the file to apply changes. Don't forget to restart the applications after the desired API availability has been applied.


"FeatureToggles": {
    "EnableTodoCreation": true,
    "EnableTodoDeletion": true,
    "EnableTodoPost": true,
    "EnableTodoPut": true,
    "EnableTodoGet": true,
    "EnableTodoGetAll": true
