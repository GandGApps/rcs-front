using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;
using Kassa.BuisnessLogic.ApplicationModelManagers;
using Kassa.BuisnessLogic.Dto;
using Kassa.BuisnessLogic.Services;
using Kassa.DataAccess.Model;
using Kassa.DataAccess.Repositories;
using Splat;

namespace Kassa.BuisnessLogic.Edgar.Services;
internal sealed class OrdersService: BaseInitializableService, IOrdersService
{
    private readonly IRepository<Order> _repository;
    private readonly IRepository<PaymentInfo> _paymentInfos;
    private readonly IShiftService _shiftService;

    public OrdersService(IRepository<Order> repository, IRepository<PaymentInfo> paymentInfos, IShiftService shiftService)
    {
        _repository = repository;
        _paymentInfos = paymentInfos;
        _shiftService = shiftService;
    }

    public IApplicationModelManager<OrderDto> RuntimeOrders
    {
        get;
    } = new HostModelManager<OrderDto>();

    protected async override ValueTask InitializeAsync(CompositeDisposable disposables)
    {
        var orders = await GetOrders();
        RuntimeOrders.AddOrUpdate(orders);
    }

    public async Task AddOrder(OrderDto order)
    {
        var model = Mapper.MapDtoToOrder(order);

        // This was supposed to be implemented by the _repository, not the service,
        // but since it's a mock interface, we have to do it here.
        if (order.Id == Guid.Empty)
        {
            order.Id = Guid.NewGuid();
        }

        if (order.PaymentInfo is PaymentInfoDto paymentInfo && paymentInfo.Id == Guid.Empty)
        {
            paymentInfo.Id = Guid.NewGuid();
            paymentInfo.OrderId = order.Id;
            order.PaymentInfo = paymentInfo;
            order.PaymentInfoId = paymentInfo.Id;

            await _paymentInfos.Add(Mapper.MapDtoToPaymentInfo(paymentInfo));
        }

        // TODO:We need to remove the code above.

        await _repository.Add(model);

        RuntimeOrders.AddOrUpdate(order);
    }
    public async Task DeleteOrder(Guid id)
    {
        var model = await _repository.Get(id);

        if (model == null)
        {
            return;
        }

        await _repository.Delete(model);
        RuntimeOrders.Remove(id);
    }

    public async ValueTask<OrderDto?> GetOrderById(Guid id)
    {
        var model = await _repository.Get(id);

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
        var models = await _repository.GetAll();

        var dtos = models.Select(Mapper.MapOrderToDto).ToList();

        RuntimeOrders.AddOrUpdate(dtos);

        return dtos;
    }

    public async Task UpdateOrder(OrderDto order)
    {
        var model = Mapper.MapDtoToOrder(order);

        // This was supposed to be implemented by the _repository, not the service,
        // but since it's a mock interface, we have to do it here.
        if (order.PaymentInfo is PaymentInfoDto paymentInfo)
        {
            paymentInfo.OrderId = order.Id;
            var paymentInfoModel = Mapper.MapDtoToPaymentInfo(paymentInfo);

            if (paymentInfo.Id == Guid.Empty)
            {
                paymentInfo.Id = Guid.NewGuid();
                paymentInfoModel.Id = paymentInfo.Id;
                order.PaymentInfoId = paymentInfo.Id;

                await _paymentInfos.Add(paymentInfoModel);
            }
            else
            {
                await _paymentInfos.Update(paymentInfoModel);
            }
        }
        // TODO:We need to remove the code above.

        RuntimeOrders.AddOrUpdate(order);

        await _repository.Update(model);
    }


    // Not sure if this is the right way to implement this method
    public Task<OrderDto> CreateOrderAsync(OrderEditDto orderEditDto)
    {
        var orderDto = CreateOrderUnsafe(orderEditDto);

        // Maybe we should add the order to the _repository here?
        return Task.FromResult(orderDto);
    }

    public OrderDto CreateOrderUnsafe(OrderEditDto orderEditDto)
    {

        var orderDto = Mapper.MapOrderEditDtoToOrderDto(orderEditDto);

        return orderDto;
    }

    public Task<IEnumerable<OrderDto>> GetOrdersOfCurrentCashierShiftAsync()
    {
        var currentCashierShift = _shiftService.CurrentShift.Value;

        if (currentCashierShift == null)
        {
            this.Log().Error("No current cashier shift found");
            throw new InvalidOperationException("No current cashier shift found");
        }

        var dto = currentCashierShift.CreateDto();
        var orders = RuntimeOrders.Values.Where(x => x.CashierShiftId == dto.Id).ToList();

        return Task.FromResult<IEnumerable<OrderDto>>(orders);
    }

    public Task<IEnumerable<OrderDto>> GetOrdersOfCurrentShiftAsync()
    {
        var currentShift = _shiftService.CurrentShift.Value;

        if (currentShift == null)
        {
            this.Log().Error("No current shift found");
            throw new InvalidOperationException("No current shift found");
        }

        var dto = currentShift.CreateDto();
        var orders = RuntimeOrders.Values.Where(x => x.ShiftId == dto.Id).ToList();

        return Task.FromResult<IEnumerable<OrderDto>>(orders);
    }
}
