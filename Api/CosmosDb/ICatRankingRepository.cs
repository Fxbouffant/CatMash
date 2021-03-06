﻿using System.Threading.Tasks;
using CatMash.Api.Interfaces;
using CatMash.Shared;

namespace CatMash.Api.CosmosDb
{
    public interface ICatRankingRepository : IRepository<CatRanking>
    {
        public Task<bool> VoteAsync(string catId);
    }
}
