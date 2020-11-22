using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using CatMash.Shared;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;

namespace CatMash.Api.CosmosDb
{
    public class CatRankingRepository : ICatRankingRepository
    {
        private readonly Container _catRankingsContainer;
        private readonly ILogger _logger;

        private const string GetAllQueryText = "SELECT * FROM Rankings";
        private const string IncrementRakingStoredProcedureName = "IncrementRanking";

        public CatRankingRepository(Container catRankingContainer, ILogger<CatRankingRepository> logger)
        {
            _catRankingsContainer = catRankingContainer;
            _logger = logger;
        }

        /// <summary>
        ///     Get all cat rankings.
        /// </summary>
        /// <returns>A List containing all cat rankings. <see cref="CatRanking"/>. Null if an error occurred.</returns>
        public async Task<List<CatRanking>> GetAllAsync()
        {
            var result = new List<CatRanking>();

            try
            {
                var options = new QueryRequestOptions { MaxBufferedItemCount = 100 };

                using FeedIterator<CatRanking> query =
                    _catRankingsContainer.GetItemQueryIterator<CatRanking>(GetAllQueryText, requestOptions: options);

                while (query.HasMoreResults)
                {
                    result.AddRange(await query.ReadNextAsync());
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unable to GetAll CatRankings.");
                return null;
            }

            return result;
        }

        /// <summary>
        ///     Get a cat ranking by catId.
        /// </summary>
        /// <param name="catId">The id of the cat.</param>
        /// <returns>The cat ranking. <see cref="CatRanking"/> Null if an error occurred.</returns>
        /// <exception cref="ArgumentException">CatId cannot be null.</exception>
        public async Task<CatRanking> GetByIdAsync(string catId)
        {
            if (catId == null)
            {
                throw new ArgumentNullException(nameof(catId));
            }

            var result = new List<CatRanking>();

            using FeedIterator<CatRanking> resultSetIterator = _catRankingsContainer.GetItemQueryIterator<CatRanking>(
                GetAllQueryText,
                requestOptions: new QueryRequestOptions
                {
                    PartitionKey = new PartitionKey(catId)
                });
            while (resultSetIterator.HasMoreResults)
            {
                result.AddRange(await resultSetIterator.ReadNextAsync());
            }

            // I've decided to have only 1 item per partitionKey for an optimal repartition of the data
            if (result.Count == 1)
            {
                return result[0];
            }

            _logger.LogError("Unable to GetByIdAsync. Duplicate records with the same catId. ({catId})", catId);
            return null;
        }

        /// <summary>
        ///     Vote for a cat.
        /// </summary>
        /// <param name="catId">The id of the cat.</param>
        /// <returns>True if the vote when through.</returns>
        /// <exception cref="ArgumentException">CatId cannot be null.</exception>
        public async Task<bool> VoteAsync(string catId)
        {
            if (catId == null)
            {
                throw new ArgumentNullException(nameof(catId));
            }

            try
            {
                string result =
                    await _catRankingsContainer.Scripts.ExecuteStoredProcedureAsync<string>(
                        IncrementRakingStoredProcedureName, new PartitionKey(catId), new dynamic[] { catId });

                if (string.IsNullOrEmpty(result))
                {
                    return true;
                }

                _logger.LogError("Unable to VoteAsync ({catId}, {result})", catId, result);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unable to VoteAsync ({catId})", catId);
            }

            return false;
        }

        /// <summary>
        ///     Upsert a CatRaking item.
        /// </summary>
        /// <param name="item">The item to upsert.</param>
        /// <returns>The created item. Null if an error occurred.</returns>
        public async Task<CatRanking> UpsertItemAsync(CatRanking item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            // PartitionKey cannot be null
            if (string.IsNullOrEmpty(item.Id))
            {
                throw new ArgumentNullException(nameof(item.Id), "Id cannot be null");
            }

            try
            {
                ItemResponse<CatRanking> response =
                    await _catRankingsContainer.UpsertItemAsync(item, new PartitionKey(item.Id));

                if (response.StatusCode == HttpStatusCode.Created)
                {
                    return response.Resource;
                }

                _logger.LogError("Unable to UpsertItemAsync ({catId}, {statusCode})",
                    item.Id, response.StatusCode);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unable to UpsertItemAsync ({catId})",
                    item.Id);
            }

            return null;
        }
    }
}
