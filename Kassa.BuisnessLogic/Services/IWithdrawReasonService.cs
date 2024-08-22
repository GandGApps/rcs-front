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
    public IApplicationModelManager<SeizureReasonDto> RuntimeWithdrawReasouns
    {
        get;
    }

    public Task AddWithdrawReasoun(SeizureReasonDto withdrawalReason);
    public Task DeleteWithdrawalReason(Guid id);
    public Task<IEnumerable<SeizureReasonDto>> GetAll();
    public ValueTask<SeizureReasonDto?> GetWithdrawalReasonById(Guid id);
    public Task UpdateWithdrawalReason(SeizureReasonDto withdrawalReason);
}
