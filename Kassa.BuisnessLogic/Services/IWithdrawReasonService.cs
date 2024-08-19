using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.BuisnessLogic.ApplicationModelManagers;
using Kassa.BuisnessLogic.Dto;

namespace Kassa.BuisnessLogic.Services;
public interface IWithdrawReasonService: IInitializableService
{
    public IApplicationModelManager<WithdrawalReasonDto> RuntimeWithdrawReasouns
    {
        get;
    }

    public Task AddWithdrawReasoun(WithdrawalReasonDto withdrawalReason);
    public Task DeleteWithdrawalReason(Guid id);
    public Task<IEnumerable<WithdrawalReasonDto>> GetAll();
    public ValueTask<WithdrawalReasonDto?> GetWithdrawalReasonById(Guid id);
    public Task UpdateWithdrawalReason(WithdrawalReasonDto withdrawalReason);
}
