using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using DynamicData;
using Kassa.BuisnessLogic.Dto;
using Kassa.DataAccess.Model;
using Kassa.DataAccess.Repositories;

namespace Kassa.BuisnessLogic.Services;
public sealed class AdditiveService(IAdditiveRepository repository, IReceiptService receiptService) : IAdditiveService
{
    public SourceCache<AdditiveDto, Guid> RuntimeAdditives
    {
        get;
    } = new(x => x.Id);

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

        if (!await receiptService.HasEnoughIngridients(receipt))
        {
            throw new ArgumentException("Additive count must be greater than zero.", nameof(additiveDto));
        }

        var foundedAdditive = await repository.Get(additiveDto.Id);

        if (foundedAdditive is null)
        {
            throw new InvalidOperationException($"Additive with id {additiveDto.Id} not found");
        }

        await receiptService.SpendIngridients(receipt);

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

        await UpdateAdditive(additiveDto);
    }

    public ValueTask Initialize()
    {
        IsInitialized = true;

        return ValueTask.CompletedTask;
    }

    public async Task UpdateAdditive(AdditiveDto additiveDto)
    {
        this.ThrowIfNotInitialized();

        RuntimeAdditives.AddOrUpdate(additiveDto);
    }
}
