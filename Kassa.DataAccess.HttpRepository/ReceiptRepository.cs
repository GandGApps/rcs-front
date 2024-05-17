using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.DataAccess.Model;
using Kassa.DataAccess.Repositories;

namespace Kassa.DataAccess.HttpRepository;
internal sealed class ReceiptRepository : IRepository<Receipt>
{
    public Task Add(Receipt item) => throw new NotImplementedException();
    public Task Delete(Receipt item) => throw new NotImplementedException();
    public Task DeleteAll() => throw new NotImplementedException();
    public Task<Receipt?> Get(Guid id) => throw new NotImplementedException();
    public Task<IEnumerable<Receipt>> GetAll() => throw new NotImplementedException();
    public Task Update(Receipt item) => throw new NotImplementedException();
}
