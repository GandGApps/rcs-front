using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicData;
using Kassa.BuisnessLogic.Dto;

namespace Kassa.BuisnessLogic.Services;
public interface IReceiptService: IInitializableService
{
    public SourceCache<ReceiptDto, Guid> RuntimeReceipts
    {
        get;
    }

    public Task SpendIngridients(ReceiptDto receiptDto, double count = 1);
    public Task ReturnIngridients(ReceiptDto receiptDto, double count = 1);
    public Task<bool> HasEnoughIngridients(ReceiptDto receiptDto, double count = 1);
    public Task AddReceipt(ReceiptDto receiptDto);
    public Task DeleteReceipt(Guid id);
    public ValueTask<ReceiptDto?> GetReceipt(Guid id);
}
