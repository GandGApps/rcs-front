using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.BuisnessLogic.ApplicationModelManagers;
using Kassa.BuisnessLogic.Dto;
using Kassa.DataAccess.Model;

namespace Kassa.BuisnessLogic.Services;
public interface IPaymentInfoService: IInitializableService
{
    public IApplicationModelManager<PaymentInfoDto> RuntimePaymentInfo
    {
        get;
    }

    public ValueTask<PaymentInfoDto?> GetPaymentInfo(Guid paymentId);

    public Task<IEnumerable<PaymentInfoDto>> GetAllPaymentInfo();

    public Task UpdatePaymentInfo(PaymentInfoDto paymentInfo);
    public Task DeletePaymentInfo(Guid paymentId);

    public Task AddPaymentInfo(PaymentInfoDto paymentInfo);
}
