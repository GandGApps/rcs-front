using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.BuisnessLogic.Dto;

namespace Kassa.BuisnessLogic.Services;

/// <summary>
/// In fact, the ingredients for the dishes are not spent until the order is fully completed,
/// but it is necessary to show the explicit lack of certain ingredients to indicate that a
/// particular product is not available for order, and to spend the ingredients only after
/// the order is completed.
/// </summary>
public interface IStorageScope : IDisposable
{

    public Task<bool> HasEnoughIngredients(ReceiptDto receiptDto, double count) =>
        HasEnoughIngredients(receiptDto.IngredientUsages, count);

    public Task SpendIngredients(ReceiptDto receiptDto, double count) =>
        SpendIngredients(receiptDto.IngredientUsages, count);

    public Task ReturnIngredients(ReceiptDto receiptDto, double count) =>
        ReturnIngredients(receiptDto.IngredientUsages, count);

    public Task<bool> HasEnoughIngredients(IEnumerable<IngredientUsageDto> ingredientUsages, double count);
    public Task SpendIngredients(IEnumerable<IngredientUsageDto> ingredientUsages, double count);
    public Task ReturnIngredients(IEnumerable<IngredientUsageDto> ingredientUsages, double count);

    /// <summary>
    /// Confirm changes
    /// </summary>
    /// <remarks>
    /// Also disposes the scope
    /// </remarks>
    public void Submit();
}
