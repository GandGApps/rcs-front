using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using DynamicData;
using Kassa.BuisnessLogic.ApplicationModelManagers;
using Kassa.BuisnessLogic.Dto;
using Kassa.DataAccess.Model;
using Kassa.DataAccess.Repositories;

namespace Kassa.BuisnessLogic.Services;
public sealed class AdditiveService(IAdditiveRepository repository, IReceiptService receiptService) : IAdditiveService
{
    public IApplicationModelManager<AdditiveDto> RuntimeAdditives
    {
        get;
    } = new HostModelManager<AdditiveDto>();

    public bool IsInitialized
    {
        get; set;
    }
    public bool IsDisposed
    {
        get; set;
    }


    public async Task DecreaseAddtiveCount(AdditiveDto additiveDto, double count = 1)
    {
        this.ThrowIfNotInitialized();
        ArgumentNullException.ThrowIfNull(additiveDto);

        var receipt = await receiptService.GetReceipt(additiveDto.ReceiptId);

        if (receipt == null)
        {
            throw new InvalidOperationException($"Receipt with id {additiveDto.ReceiptId} not found");
        }

        if (!await receiptService.HasEnoughIngridients(receipt, count))
        {
            throw new InvalidOperationException($"Additive with id {additiveDto.Id} has not enough count");
        }

        var foundedAdditive = await repository.Get(additiveDto.Id);

        if (foundedAdditive is null)
        {
            throw new InvalidOperationException($"Additive with id {additiveDto.Id} not found");
        }

        await receiptService.SpendIngridients(receipt, count);

        await CheckAllIngridients((foundedAdditive, receipt));
        await UpdateAdditive(Mapper.MapAdditiveToAdditiveDto(foundedAdditive));
    }
    public void Dispose()
    {
        RuntimeAdditives.Dispose();
        IsDisposed = true;
    }

    public ValueTask DisposeAsync()
    {
        Dispose();

        return ValueTask.CompletedTask;
    }

    public async ValueTask<AdditiveDto?> GetAdditiveById(Guid additiveId)
    {
        this.ThrowIfNotInitialized();

        var foundedAdditive = await repository.Get(additiveId);

        if (foundedAdditive is null)
        {
            return null;
        }

        var dto = Mapper.MapAdditiveToAdditiveDto(foundedAdditive);

        RuntimeAdditives.AddOrUpdate(dto);

        return dto;
    }

    public async ValueTask<IEnumerable<AdditiveDto>> GetAdditivesByProductId(Guid id)
    {
        this.ThrowIfNotInitialized();

        var additives = await repository.GetAdditivesByProductId(id);
        var additivesDto = additives.Select(Mapper.MapAdditiveToAdditiveDto);

        RuntimeAdditives.AddOrUpdate(additivesDto);

        return additivesDto;
    }

    public async Task IncreaseAdditiveCount(AdditiveDto additiveDto, double count = 1)
    {
        this.ThrowIfNotInitialized();

        var foundedAdditive = await repository.Get(additiveDto.Id);

        if (foundedAdditive is null)
        {
            throw new InvalidOperationException($"Additive with id {additiveDto.Id} not found");
        }

        var receipt = await receiptService.GetReceipt(additiveDto.ReceiptId);

        if (receipt is null)
        {
            throw new InvalidOperationException($"Receipt with id {additiveDto.ReceiptId} not found");
        }

        if (!await receiptService.HasEnoughIngridients(receipt, count))
        {
            throw new InvalidOperationException($"Additive with id {additiveDto.Id} has not enough count");
        }

        await receiptService.ReturnIngridients(receipt, count);

        await CheckAllIngridients((foundedAdditive, receipt));

        additiveDto.IsEnoughIngredients = await receiptService.HasEnoughIngridients(receipt, 1);

        await UpdateAdditive(additiveDto);
    }

    public async ValueTask Initialize()
    {
        if (IsInitialized)
        {
            return;
        }

        IsInitialized = true;

        await receiptService.Initialize();

        var additives = await repository.GetAll();

        foreach (var additive in additives)
        {
            var additiveDto = Mapper.MapAdditiveToAdditiveDto(additive);

            var receipt = await receiptService.GetReceipt(additiveDto.ReceiptId);

            if (receipt is null)
            {
                throw new InvalidOperationException($"Receipt with id {additiveDto.ReceiptId} not found");
            }

            additiveDto.IsEnoughIngredients = await receiptService.HasEnoughIngridients(receipt, 1);

            await UpdateAdditive(additiveDto);

        }
    }

    public async Task UpdateAdditive(AdditiveDto additiveDto)
    {
        this.ThrowIfNotInitialized();

        var model = repository.Get(additiveDto.Id);

        if (model is null)
        {
            throw new ArgumentNullException(nameof(additiveDto));
        }

        await repository.Update(Mapper.MapAdditiveDtoToAdditive(additiveDto));

        RuntimeAdditives.AddOrUpdate(additiveDto);
    }

    private async Task CheckAllIngridients((Additive product, ReceiptDto receipt)? existingModel)
    {
        var products = RuntimeAdditives.Values;

        if (existingModel.HasValue)
        {
            var (product, receipt) = existingModel.Value;
            product.IsEnoughIngredients = await receiptService.HasEnoughIngridients(receipt, 1);
        }


        foreach (var product in products)
        {
            if (existingModel.HasValue && existingModel.Value.product.Id == product.Id)
            {
                continue;
            }

            var receipt = await receiptService.GetReceipt(product.ReceiptId);

            if (receipt is null)
            {
                throw new InvalidOperationException($"Receipt with id {product.ReceiptId} not found");
            }

            product.IsEnoughIngredients = await receiptService.HasEnoughIngridients(receipt, 1);

            await UpdateAdditive(product);
        }
    }
}
