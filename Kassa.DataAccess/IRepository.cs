using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kassa.DataAccess;
public interface IRepository<T> where T : class
{
    public Task<T> Get(int categoryId);
    public Task<IEnumerable<T>> GetAll();
    public Task Add(T item);
    public Task Update(T item);
    public Task Delete(T item);
    public Task DeleteAll();
}
