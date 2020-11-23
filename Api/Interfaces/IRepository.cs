using System.Collections.Generic;
using System.Threading.Tasks;

namespace CatMash.Api.Interfaces
{
    public interface IRepository<T>
    {
        public Task<List<T>> GetAllAsync();

        public Task<T> GetByIdAsync(string id);

        public Task<T> UpsertItemAsync(T item);
    }
}
