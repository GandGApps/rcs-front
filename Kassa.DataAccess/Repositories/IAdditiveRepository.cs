using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.DataAccess.Model;

namespace Kassa.DataAccess.Repositories;
public interface IAdditiveRepository : IRepository<Additive>
{
    public Task<IEnumerable<Additive>> GetAdditivesByProductId(Guid productId);

    internal class MockAdditveRepository(IRepository<Additive> repository) : IAdditiveRepository
    {
        public Task Add(Additive item) => repository.Add(item);
        public Task Delete(Additive item) => repository.Delete(item);
        public Task DeleteAll() => repository.DeleteAll();
        public Task<Additive?> Get(Guid categoryId) => repository.Get(categoryId);
        public async Task<IEnumerable<Additive>> GetAdditivesByProductId(Guid productId)
        {
            var additives = await repository.GetAll();

            return additives.Where(additive => additive.ProductIds.Contains(productId));
        }

        public Task<IEnumerable<Additive>> GetAll() => repository.GetAll();
        public Task Update(Additive item) => repository.Update(item);
    }
}
