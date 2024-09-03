using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.BuisnessLogic.ApplicationModelManagers;
using Kassa.BuisnessLogic.Dto;


namespace Kassa.BuisnessLogic.Services;
public interface IOrdersService: IInitializableService
{
    public IApplicationModelManager<OrderDto> RuntimeOrders
    {
        get;
    }

    public Task<IEnumerable<OrderDto>> GetOrdersOfCurrentCashierShiftAsync();
    public Task<IEnumerable<OrderDto>> GetOrdersOfCurrentShiftAsync();

    public Task<OrderDto> CreateOrderAsync(OrderEditDto orderEditDto);

    /// <summary>
    /// Here just convert or map OrderEditDto to OrderDto, without using <see cref="DataAccess.Repositories.IRepository{T}"/>
    /// </summary>
    public OrderDto CreateOrderUnsafe(OrderEditDto orderEditDto);
    public ValueTask<OrderDto?> GetOrderById(Guid id);
    public Task UpdateOrder(OrderDto order);
    public Task AddOrder(OrderDto order);
    public Task DeleteOrder(Guid id);
    public Task<IEnumerable<OrderDto>> GetOrders();
}
