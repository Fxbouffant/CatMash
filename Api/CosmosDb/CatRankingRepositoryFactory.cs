using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;

namespace CatMash.Api.CosmosDb
{
    public class CatRankingRepositoryFactory
    {
        private const string DatabaseId = "cat-mash";
        private const string ContainerId = "Rankings";
        private const string PartitionKeyPath = "/catid";

        private readonly CosmosClient _cosmosClient;
        private readonly ILogger<CatRankingRepository> _logger;

        private ICatRankingRepository _catRankingRepository;

        public CatRankingRepositoryFactory(CosmosClient cosmosClient, ILogger<CatRankingRepository> logger)
        {
            _cosmosClient = cosmosClient;
            _logger = logger;
        }

        /// <summary>
        ///     Create a CatRankingRepository. 
        /// </summary>
        /// <returns>The created CatRankingRepository or the existing one if already created. Null if an error occurred.</returns>
        public async Task<ICatRankingRepository> CreateCatRankingRepositoryAsync()
        {
            if (_catRankingRepository != null)
            {
                return _catRankingRepository;
            }

            try
            {
                DatabaseResponse dbResponse = await _cosmosClient.CreateDatabaseIfNotExistsAsync(DatabaseId);
                if (dbResponse.StatusCode == HttpStatusCode.Accepted || dbResponse.StatusCode == HttpStatusCode.Created || dbResponse.StatusCode == HttpStatusCode.OK)
                {
                    ContainerResponse containerResponse =
                        await dbResponse.Database.CreateContainerIfNotExistsAsync(ContainerId, PartitionKeyPath);

                    if (containerResponse.StatusCode == HttpStatusCode.Accepted || containerResponse.StatusCode == HttpStatusCode.Created || containerResponse.StatusCode == HttpStatusCode.OK)

                    {
                        _catRankingRepository =
                            new CatRankingRepository(_cosmosClient.GetContainer(DatabaseId, ContainerId), _logger);
                        return _catRankingRepository;
                    }

                    _logger.LogError(
                        "Unable to CreateContainerIfNotExistsAsync. ({containerId}, {partitionKeyPath}, {statusCode})",
                        ContainerId, PartitionKeyPath, containerResponse.StatusCode);
                    return null;
                }

                _logger.LogError("Unable to CreateDatabaseIfNotExistsAsync. ({databaseId}, {statusCode})",
                    DatabaseId, dbResponse.StatusCode);
                return null;

            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unable to CreateCatRankingRepositoryAsync");
                return null;
            }
        }
    }
}
