using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.DataAccess.HttpRepository.Api;
using Kassa.DataAccess.Model;
using Kassa.DataAccess.Repositories;
using Kassa.Shared;
using Splat;

namespace Kassa.DataAccess.HttpRepository;
internal sealed class OrderRepository : IRepository<Order>
{
    public async Task Add(Order item)
    {
        var orderApi = Locator.Current.GetRequiredService<IOrdersApi>();

        var request = ApiMapper.MapOrderToEdgarModel(item);

        await orderApi.AddOrder(request);
    }
    public Task Delete(Order item) => throw new NotImplementedException();
    public Task DeleteAll() => throw new NotImplementedException();
    public Task<Order?> Get(Guid id) => throw new NotImplementedException();
    public async Task<IEnumerable<Order>> GetAll()
    {
        var orderApi = Locator.Current.GetRequiredService<IOrdersApi>();

        var orders = await orderApi.GetOrders();

        if (orders == null)
        {
            return [];
        }

        return orders.Select(ApiMapper.MapServerResponseToOrder).ToList();
    }
    public Task Update(Order item) => throw new NotImplementedException();
}
