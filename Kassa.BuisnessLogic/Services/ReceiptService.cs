using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using DynamicData;
using Kassa.BuisnessLogic.Dto;
using Kassa.DataAccess.Model;
using Kassa.DataAccess.Repositories;

namespace Kassa.BuisnessLogic.Services;
internal sealed class ReceiptService(IRepository<Receipt> repository, IIngridientsService ingridientsService) : BaseInitializableService, IReceiptService
{
    public SourceCache<ReceiptDto, Guid> RuntimeReceipts
    {
        get;
    } = new(x => x.Id);

    protected async override ValueTask InitializeAsync(CompositeDisposable disposables)
    {
        await ingridientsService.Initialize();

        var receipts = await repository.GetAll();

        foreach (var receipt in receipts)
        {
            var receiptDto = Mapper.MapReceiptToDto(receipt);

            RuntimeReceipts.AddOrUpdate(receiptDto);
        }
    }

    public async Task AddReceipt(ReceiptDto receiptDto)
    {
        var model = Mapper.MapDtoToReceipt(receiptDto);

        await repository.Add(model);
    }

    public async Task DeleteReceipt(Guid id)
    {
        var model = await repository.Get(id);

        if (model == null)
        {
            throw new InvalidOperationException($"Receipt with id {id} not found");
        }

        await repository.Delete(model);
    }

    public async ValueTask<ReceiptDto?> GetReceipt(Guid id)
    {
        var model = await repository.Get(id);

        return model == null ? null : Mapper.MapReceiptToDto(model);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public Task<bool> HasEnoughIngridients(ReceiptDto receiptDto, double count = 1)
    {
        return ingridientsService.HasEnoughIngredients(receiptDto.IngredientUsages, count);
    }

    public Task ReturnIngridients(ReceiptDto receiptDto, double count = 1)
    {
        return ingridientsService.ReturnIngridients(receiptDto.IngredientUsages, count);
    }

    public Task SpendIngridients(ReceiptDto receiptDto, double count = 1) => ingridientsService.SpendIngridients(receiptDto.IngredientUsages, count);
}
