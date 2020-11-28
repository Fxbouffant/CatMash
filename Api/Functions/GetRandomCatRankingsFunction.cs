using System.Collections.Generic;
using System.Linq;
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
    public class GetRandomCatRankingsFunction
    {
        private readonly CatRankingRepositoryFactory _catRankingRepositoryFactory;

        public GetRandomCatRankingsFunction(CatRankingRepositoryFactory catRankingRepositoryFactory)
        {
            _catRankingRepositoryFactory = catRankingRepositoryFactory;
        }

        [FunctionName("GetRandomCatRankingsFunction")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "random")] HttpRequest req,
            ILogger logger)
        {
            logger.LogInformation("Start GetRandomCatRankingsFunction");
            
            ICatRankingRepository catRankingRepository = await _catRankingRepositoryFactory.CreateCatRankingRepositoryAsync();
            if (catRankingRepository == null)
            {
                return new InternalServerErrorResult();
            }

            List<CatRanking> rankings = await catRankingRepository.GetAllAsync();
            if (rankings == null)
            {
                return new InternalServerErrorResult();
            }
            
            rankings.Shuffle();

            // We don't want to expose cats without images
            rankings = rankings.Where(r => r.ImageUrl != null).Take(2).ToList();

            if (rankings.Count < 2)
            {
                logger.LogError("Unable to get 2 random CatRankings with an ImageUrl");
                return new InternalServerErrorResult();
            }

            logger.LogInformation("Found 2 RandomCatRanking. ({firstCatRankingId}, {secondCatRankingId})",
                rankings[0].CatId, rankings[1].CatId);

            return new OkObjectResult(rankings);
        }
    }
}
