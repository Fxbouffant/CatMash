using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CatMash.Api;
using CatMash.Api.CosmosDb;
using CatMash.Shared;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Api.Tests.IT
{
    public class RepositoryFixture : IDisposable
    {
        public RepositoryFixture()
        {
            string connectionString =
                Environment.GetEnvironmentVariable(Startup.CosmosDbConnectionStringEnv, EnvironmentVariableTarget.User);
            
            var client = new CosmosClient(connectionString);

            RepositoryFactory = new CatRankingRepositoryFactory(client, new Mock<ILogger<CatRankingRepository>>().Object);
        }

        public void Dispose()
        {

        }

        public CatRankingRepositoryFactory RepositoryFactory;
    }

    public class CatRankingRepositoryTest : IClassFixture<RepositoryFixture>
    {
        private const string TestPrefix = "TEST-";

        private RepositoryFixture _fixture;

        public CatRankingRepositoryTest(RepositoryFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task UpsertTest()
        {
            CatRankingRepository repository = await _fixture.RepositoryFactory.CreateCatRankingRepositoryAsync();

            CatRanking catRanking = await repository.UpsertItemAsync(new CatRanking
            {
                CatId = $"{TestPrefix}1",
                VoteCount = 0
            });

            Assert.NotNull(catRanking);
        }

        [Fact]
        public async Task GetByIdTest()
        {
            CatRankingRepository repository = await _fixture.RepositoryFactory.CreateCatRankingRepositoryAsync();

            CatRanking catRanking = await repository.UpsertItemAsync(new CatRanking
            {
                CatId = $"{TestPrefix}1",
                VoteCount = 0
            });

            Assert.NotNull(catRanking);

            catRanking = await repository.GetByIdAsync(catRanking.CatId);

            Assert.NotNull(catRanking);
        }

        [Fact]
        public async Task GetAllTest()
        {
            CatRankingRepository repository = await _fixture.RepositoryFactory.CreateCatRankingRepositoryAsync();

            for (int i = 0; i < 5; i++)
            {
                CatRanking catRanking = await repository.UpsertItemAsync(new CatRanking
                {
                    CatId = $"{TestPrefix}{i}",
                    VoteCount = 0
                });

                Assert.NotNull(catRanking);
            }

            List<CatRanking> catRankings = await repository.GetAllAsync();
            
            Assert.Equal(5, catRankings.Count(c => c.CatId.StartsWith(TestPrefix)));
        }

        [Fact]
        public async Task VoteTest()
        {
            CatRankingRepository repository = await _fixture.RepositoryFactory.CreateCatRankingRepositoryAsync();

            CatRanking catRanking = await repository.UpsertItemAsync(new CatRanking
            {
                CatId = $"{TestPrefix}1",
                VoteCount = 0
            });
            
            bool result = await repository.VoteAsync(catRanking.CatId);
            catRanking = await repository.GetByIdAsync(catRanking.CatId);
            
            Assert.True(result);
            Assert.NotNull(catRanking);
            Assert.Equal(1, catRanking.VoteCount);
        }

    }
}
