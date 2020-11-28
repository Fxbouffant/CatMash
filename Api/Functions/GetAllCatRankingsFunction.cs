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
    public class GetAllCatRankingsFunction
    {
        private readonly CatRankingRepositoryFactory _catRankingRepositoryFactory;

        public GetAllCatRankingsFunction(CatRankingRepositoryFactory catRankingRepositoryFactory)
        {
            _catRankingRepositoryFactory = catRankingRepositoryFactory;
        }

        [FunctionName("GetAllCatRankingsFunction")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "rankings")] HttpRequest req,
            ILogger logger)
        {
            logger.LogInformation("Start GetAllCatRankingsFunction");

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

            // We don't want to expose cats without images
            rankings = rankings.Where(r => r.ImageUrl != null).OrderByDescending(r => r.VoteCount).ToList();

            logger.LogInformation("GetAllCatRankingsFunction: Found rankings. ({count})", rankings.Count);
            if (rankings.Count == 0)
            {
                return new NotFoundResult();
            }

            return new OkObjectResult(rankings);
        }
    }
}
