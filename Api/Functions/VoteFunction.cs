using System.Threading.Tasks;
using System.Web.Http;
using CatMash.Api.CosmosDb;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace CatMash.Api.Functions
{
    public class VoteFunction
    {
        private readonly CatRankingRepositoryFactory _catRankingRepositoryFactory;

        public VoteFunction(CatRankingRepositoryFactory catRankingRepositoryFactory)
        {
            _catRankingRepositoryFactory = catRankingRepositoryFactory;
        }

        [FunctionName("VoteFunction")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "vote/{catId}")] HttpRequest req,
            string catId, ILogger logger)
        {
            logger.LogInformation("Start VoteFunction ({catId})", catId);

            ICatRankingRepository catRankingRepository = await _catRankingRepositoryFactory.CreateCatRankingRepositoryAsync();
            if (catRankingRepository == null)
            {
                return new InternalServerErrorResult();
            }

            bool result = await catRankingRepository.VoteAsync(catId);
            if (!result)
            {
                return new InternalServerErrorResult();
            }

            logger.LogInformation("VoteFunction: Vote went through. ({catId})", catId);

            return new OkResult();
        }
    }
}
