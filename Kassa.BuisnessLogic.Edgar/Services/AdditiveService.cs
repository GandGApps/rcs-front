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
internal sealed class AdditiveService : BaseInitializableService, IAdditiveService
{
    private readonly IAdditiveRepository _repository;
    private readonly IReceiptService _receiptService;

    public AdditiveService(IAdditiveRepository repository, IReceiptService receiptService)
    {
        _repository = repository;
        _receiptService = receiptService;
    }

    public IApplicationModelManager<AdditiveDto> RuntimeAdditives
    {
        get;
    } = new HostModelManager<AdditiveDto>();

    public async Task DecreaseAddtiveCount(AdditiveDto additiveDto, double count = 1)
    {
        this.ThrowIfNotInitialized();
        ArgumentNullException.ThrowIfNull(additiveDto);

        var receipt = await _receiptService.GetReceipt(additiveDto.ReceiptId);

        if (receipt != null)
        {
            if (!await _receiptService.HasEnoughIngridients(receipt, count))
            {
                throw new InvalidOperationException($"Additive with id {additiveDto.Id} has not enough count");
            }

            var foundedAdditive = await _repository.Get(additiveDto.Id);

            if (foundedAdditive is null)
            {
                throw new InvalidOperationException($"Additive with id {additiveDto.Id} not found");
            }

            await _receiptService.SpendIngridients(receipt, count);

            await CheckAllIngridients((foundedAdditive, receipt));
            await UpdateAdditive(Mapper.MapAdditiveToAdditiveDto(foundedAdditive));
        }
    }

    public async ValueTask<AdditiveDto?> GetAdditiveById(Guid additiveId)
    {
        this.ThrowIfNotInitialized();

        var foundedAdditive = await _repository.Get(additiveId);

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

        var additives = await _repository.GetAdditivesByProductId(id);
        var additivesDto = additives.Select(Mapper.MapAdditiveToAdditiveDto);

        RuntimeAdditives.AddOrUpdate(additivesDto);

        return additivesDto;
    }

    public async Task IncreaseAdditiveCount(AdditiveDto additiveDto, double count = 1)
    {
        this.ThrowIfNotInitialized();

        var foundedAdditive = await _repository.Get(additiveDto.Id);

        if (foundedAdditive is null)
        {
            throw new InvalidOperationException($"Additive with id {additiveDto.Id} not found");
        }

        var receipt = await _receiptService.GetReceipt(additiveDto.ReceiptId);

        if (receipt is not null)
        {
            if (!await _receiptService.HasEnoughIngridients(receipt, count))
            {
                throw new InvalidOperationException($"Additive with id {additiveDto.Id} has not enough count");
            }

            await _receiptService.ReturnIngridients(receipt, count);

            await CheckAllIngridients((foundedAdditive, receipt));

            additiveDto.IsEnoughIngredients = await _receiptService.HasEnoughIngridients(receipt, 1);

            await UpdateAdditive(additiveDto);
        }
    }

    protected async override ValueTask InitializeAsync(CompositeDisposable disposables)
    {
        await _receiptService.Initialize();

        var additives = await _repository.GetAll();

        foreach (var additive in additives)
        {
            var additiveDto = Mapper.MapAdditiveToAdditiveDto(additive);

            var receipt = await _receiptService.GetReceipt(additiveDto.ReceiptId);

            if (receipt is null)
            {
                this.Log().Error($"Receipt with id {additiveDto.ReceiptId} not found");
                continue;
            }

            additiveDto.IsEnoughIngredients = await _receiptService.HasEnoughIngridients(receipt, 1);

            await UpdateAdditive(additiveDto);

        }
    }

    public async Task UpdateAdditive(AdditiveDto additiveDto)
    {
        var model = _repository.Get(additiveDto.Id);

        if (model is null)
        {
            throw new ArgumentNullException(nameof(additiveDto));
        }

        await _repository.Update(Mapper.MapAdditiveDtoToAdditive(additiveDto));

        RuntimeAdditives.AddOrUpdate(additiveDto);
    }

    private async Task CheckAllIngridients((Additive product, ReceiptDto receipt)? existingModel)
    {
        var products = RuntimeAdditives.Values;

        if (existingModel.HasValue)
        {
            var (product, receipt) = existingModel.Value;
            product.IsEnoughIngredients = await _receiptService.HasEnoughIngridients(receipt, 1);
        }


        foreach (var product in products)
        {
            if (existingModel.HasValue && existingModel.Value.product.Id == product.Id)
            {
                continue;
            }

            var receipt = await _receiptService.GetReceipt(product.ReceiptId);

            if (receipt is null)
            {
                throw new InvalidOperationException($"Receipt with id {product.ReceiptId} not found");
            }

            product.IsEnoughIngredients = await _receiptService.HasEnoughIngridients(receipt, 1);

            await UpdateAdditive(product);
        }
    }

}
