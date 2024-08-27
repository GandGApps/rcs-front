using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;
using Kassa.BuisnessLogic.ApplicationModelManagers;
using Kassa.BuisnessLogic.Dto;
using Kassa.DataAccess.Model;
using Kassa.DataAccess.Repositories;

namespace Kassa.BuisnessLogic.Services;
internal class PaymentInfoService(IRepository<PaymentInfo> repository) : BaseInitializableService, IPaymentInfoService
{

    public IApplicationModelManager<PaymentInfoDto> RuntimePaymentInfo
    {
        get;
    } = new HostModelManager<PaymentInfoDto>();

    public async ValueTask<PaymentInfoDto?> GetPaymentInfo(Guid paymentId)
    {
        var paymentInfo = await repository.Get(paymentId);

        if (paymentInfo == null)
        {
            return null;
        }

        var dto = Mapper.MapPaymentInfoToDto(paymentInfo);

        RuntimePaymentInfo.AddOrUpdate(dto);

        return dto;
    }

    public async Task<IEnumerable<PaymentInfoDto>> GetAllPaymentInfo()
    {
        var models = await repository.GetAll();
        var dtos = models.Select(Mapper.MapPaymentInfoToDto);

        RuntimePaymentInfo.AddOrUpdate(dtos);

        return dtos;
    }

    public async Task UpdatePaymentInfo(PaymentInfoDto paymentInfo)
    {
        var model = await repository.Get(paymentInfo.Id);

        if (model == null)
        {
            throw new InvalidOperationException("Payment info not found");
        }
        var updatedModel = Mapper.MapDtoToPaymentInfo(paymentInfo);

        RuntimePaymentInfo.AddOrUpdate(paymentInfo);
        await repository.Update(updatedModel);
    }

    public async Task DeletePaymentInfo(Guid paymentId)
    {

        var paymentInfo = await repository.Get(paymentId);

        if (paymentInfo == null)
        {
            throw new InvalidOperationException("Payment info not found");
        }

        RuntimePaymentInfo.Remove(paymentId);

        await repository.Delete(paymentInfo);
    }

    public async Task AddPaymentInfo(PaymentInfoDto paymentInfo)
    {
        paymentInfo.Id = paymentInfo.Id == Guid.Empty ? Guid.NewGuid() : paymentInfo.Id;

        var model = Mapper.MapDtoToPaymentInfo(paymentInfo);

        await repository.Add(model);

        RuntimePaymentInfo.AddOrUpdate(paymentInfo);
    }

    protected async override ValueTask InitializeAsync(CompositeDisposable disposables)
    {
        var models = await repository.GetAll();

        RuntimePaymentInfo.AddOrUpdate(models.Select(Mapper.MapPaymentInfoToDto));

    }

}
