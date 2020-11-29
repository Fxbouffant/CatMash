# CatMash

This is a small web application which is based on the Facemash project but for cute cats. It uses data from https://latelier.co/data/cats.json.

There are two pages:
* **The index page "https://mango-dune-0263ed003.azurestaticapps.net/"**: allows the user to vote for a cat
* **The rankings pages "https://mango-dune-0263ed003.azurestaticapps.net/rankings"**: allows the user to see the cat rankings

## Project Structure

The project was created using Visual Studio 2019  
* **Client**: The Blazor WebAssembly application
* **API**: A C# Azure Functions API, which the Blazor application will call
* **API.Tests.IT**: Integration tests for the CatRankingsRepository
* **Shared**: A C# class library with a shared data model between the Blazor and Functions application

## Technologies used

This project is based on Azure Static Web App https://azure.microsoft.com/en-us/services/app-service/static/  
You can get the template here: https://github.com/staticwebdev/blazor-starter  

Front-End Javascript is generated using Blazor https://dotnet.microsoft.com/apps/aspnet/web-apps/blazor  
The Backend API is build using Azure Functions https://azure.microsoft.com/en-us/services/functions/  
CosmosDB is used for the database https://azure.microsoft.com/en-us/services/cosmos-db/  
