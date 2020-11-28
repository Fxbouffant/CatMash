using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web.Http;
using CatMash.Api.CosmosDb;
using CatMash.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace CatMash.Api.Functions
{
    public class InitDbFunction
    {
        private const string DataUrl = "https://latelier.co/data/cats.json";
        private readonly HttpClient _httpClient;
        private readonly CatRankingRepositoryFactory _catRankingRepositoryFactory;

        public InitDbFunction(HttpClient httpClient, CatRankingRepositoryFactory catRankingRepositoryFactory)
        {
            _httpClient = httpClient;
            _catRankingRepositoryFactory = catRankingRepositoryFactory;
        }

        /// <summary>
        ///     Fetches data from DataUrl, maps and sends it to CosmosDB.
        ///     This function will reset the vote count.
        /// </summary>
        [FunctionName("InitDbFunction")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "initDb")] HttpRequest req,
            ILogger log)
        {
            HttpResponseMessage catDataResponse = null;

            try
            {
                catDataResponse = await _httpClient.GetAsync(new Uri(DataUrl));
                catDataResponse.EnsureSuccessStatusCode();
            }
            catch (Exception e)
            {
                log.LogError(e, "Unable to fetch data. ({DataUrl}, {StatusCode})", DataUrl, catDataResponse?.StatusCode);
                return new InternalServerErrorResult();
            }

            ImageContainer imageContainer;
            try
            {
                imageContainer = JsonSerializer.Deserialize<ImageContainer>(await catDataResponse.Content.ReadAsStringAsync());
            }
            catch (Exception e)
            {
                log.LogError(e, "Unable to deserialize fetched data.");
                return new InternalServerErrorResult();
            }

            ICatRankingRepository repository = await _catRankingRepositoryFactory.CreateCatRankingRepositoryAsync();

            if (repository == null)
            {
                return new InternalServerErrorResult();
            }

            foreach (Image image in imageContainer.Images)
            {
                if (image == null)
                {
                    continue;
                }

                CatRanking result = await repository.UpsertItemAsync(new CatRanking
                {
                    CatId = image.Id,
                    ImageUrl = image.Url,
                    VoteCount = 0
                });

                if (result == null)
                {
                    return new InternalServerErrorResult();
                }
            }

            return new OkResult();
        }
    }
}
