using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.BuisnessLogic.ApplicationModelManagers;
using Kassa.BuisnessLogic.Dto;
using Kassa.DataAccess.Model;
using Kassa.DataAccess.Repositories;

namespace Kassa.BuisnessLogic.Services;
internal class OrdersService(IRepository<Order> repository) : BaseInitializableService, IOrdersService
{
    public IApplicationModelManager<OrderDto> RuntimeOrders
    {
        get;
    } = new HostModelManager<OrderDto>();

    public async Task AddOrder(OrderDto order)
    {
        var model = Mapper.MapDtoToOrder(order);

        await repository.Add(model);

        RuntimeOrders.AddOrUpdate(order);
    }
    public async Task DeleteOrder(Guid id)
    {
        var model = await repository.Get(id);

        if (model == null)
        {
            return;
        }

        await repository.Delete(model);
        RuntimeOrders.Remove(id);
    }

    public async ValueTask<OrderDto?> GetOrderById(Guid id)
    {
        var model = await repository.Get(id);

        if (model == null)
        {
            return null;
        }

        var dto = Mapper.MapOrderToDto(model);
        RuntimeOrders.AddOrUpdate(dto);

        return dto;
    }

    public async Task<IEnumerable<OrderDto>> GetOrders()
    {
        var models = await repository.GetAll();

        var dtos = models.Select(Mapper.MapOrderToDto).ToList();

        RuntimeOrders.AddOrUpdate(dtos);

        return dtos;
    }

    public Task UpdateOrder(OrderDto client)
    {
        var model = Mapper.MapDtoToOrder(client);

        RuntimeOrders.AddOrUpdate(client);

        return repository.Update(model);
    }
}
