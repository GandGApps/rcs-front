using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kassa.DataAccess;
public interface IAdditiveRepository: IRepository<Additive>
{
    public Task<IEnumerable<Additive>> GetAdditivesByProductId(int productId);


    internal class MockAdditveRepository(IRepository<Additive> repository) : IAdditiveRepository
    {
        public Task Add(Additive item) => repository.Add(item);
        public Task Delete(Additive item) => repository.Delete(item);
        public Task DeleteAll() => repository.DeleteAll();
        public Task<Additive?> Get(int categoryId) => repository.Get(categoryId);
        public async Task<IEnumerable<Additive>> GetAdditivesByProductId(int productId)
        {
            var additives = await repository.GetAll();

            return additives.Where(additive => additive.ProductIds.Contains(productId));
        }
        public Task<IEnumerable<Additive>> GetAll() => repository.GetAll();
        public Task Update(Additive item) => repository.Update(item);
    }
}
