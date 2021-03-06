﻿using System;
using System.Configuration;
using System.Net.Http;
using CatMash.Api.CosmosDb;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(CatMash.Api.Startup))]
namespace CatMash.Api
{
    public class Startup : FunctionsStartup
    {
        public const string CosmosDbConnectionStringEnv = "COSMOS_DB_CONNECTION_STRING";

        public override void Configure(IFunctionsHostBuilder builder)
        {
            // Get the CosmosDbConnectionString from the environment
            string cosmosDbConnectionString =
                Environment.GetEnvironmentVariable(CosmosDbConnectionStringEnv, EnvironmentVariableTarget.Process);

            if (string.IsNullOrEmpty(cosmosDbConnectionString))
            {
                throw new ConfigurationErrorsException($"{CosmosDbConnectionStringEnv} environment variable not found.");
            }

            builder.Services.AddLogging();
            builder.Services.AddSingleton<CosmosClient>(new CosmosClient(cosmosDbConnectionString, new CosmosClientOptions
            {
                ApplicationName = nameof(CatMash.Api)
            }));
            builder.Services.AddSingleton<CatRankingRepositoryFactory>();
            builder.Services.AddSingleton(new HttpClient());
        }
    }
}
