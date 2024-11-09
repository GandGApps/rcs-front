using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Diagnostics;
using Kassa.DataAccess.HttpRepository.Api;
using Kassa.DataAccess.Model;
using Kassa.DataAccess.Repositories;
using Kassa.Shared;
using Splat;

namespace Kassa.DataAccess.HttpRepository;
internal sealed class OrderRepository : IRepository<Order>
{
    private readonly IOrdersApi _ordersApi;
    public OrderRepository(IOrdersApi ordersApi)
    {
        _ordersApi = ordersApi;
    }

    public async Task Add(Order item)
    {
        var request = ApiMapper.MapOrderToEdgarModel(item);

        await _ordersApi.AddOrder(request);
    }
    public Task Delete(Order item) => ThrowHelper.ThrowNotSupportedException<Task>();
    public Task DeleteAll() => ThrowHelper.ThrowNotSupportedException<Task>();
    public Task<Order?> Get(Guid id) => ThrowHelper.ThrowNotSupportedException<Task<Order?>>();
    public async Task<IEnumerable<Order>> GetAll()
    {
        var orders = await _ordersApi.GetOrders();

        if (orders == null)
        {
            return [];
        }

        return orders.Select(ApiMapper.MapServerResponseToOrder).ToList();
    }
    public Task Update(Order item) => ThrowHelper.ThrowNotSupportedException<Task>();
}
