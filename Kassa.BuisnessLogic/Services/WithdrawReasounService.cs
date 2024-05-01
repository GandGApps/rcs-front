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
internal class WithdrawReasounService(IRepository<WithdrawalReason> repository) : BaseInitializableService, IWithdrawReasounService
{
    private readonly HostModelManager<WithdrawalReasonDto> _hostModelManager = new();

    public IApplicationModelManager<WithdrawalReasonDto> RuntimeWithdrawReasouns => _hostModelManager;

    public async Task AddWithdrawReasoun(WithdrawalReasonDto withdrawalReason)
    {
        var model = Mapper.MapDtoToWithdrawalReason(withdrawalReason);

        await repository.Add(model);

        var dto = Mapper.MapWithdrawalReasonToDto(model);

        _hostModelManager.AddOrUpdate(dto);
    }

    public async Task DeleteWithdrawalReason(Guid id)
    {
        var withdrawalReason = await repository.Get(id);

        if (withdrawalReason is null)
        {
            throw new InvalidOperationException($"Withdrawal reason with id {id} not found");
        }

        await repository.Delete(withdrawalReason);

        _hostModelManager.Remove(id);
    }

    public async ValueTask<WithdrawalReasonDto?> GetWithdrawalReasonById(Guid id)
    {
        var withdrawalReason = await repository.Get(id);

        if (withdrawalReason is null)
        {
            return null;
        }

        var dto = Mapper.MapWithdrawalReasonToDto(withdrawalReason);
        _hostModelManager.AddOrUpdate(dto);

        return dto;
    }

    public async Task UpdateWithdrawalReason(WithdrawalReasonDto withdrawalReason)
    {
        var model = Mapper.MapDtoToWithdrawalReason(withdrawalReason);

        await repository.Update(model);

        var dto = Mapper.MapWithdrawalReasonToDto(model);

        _hostModelManager.AddOrUpdate(dto);
    }

    public async Task<IEnumerable<WithdrawalReasonDto>> GetAll()
    {
        var withdrawalReasons = (await repository.GetAll()).Select(Mapper.MapWithdrawalReasonToDto);

        _hostModelManager.AddOrUpdate(withdrawalReasons);

        return withdrawalReasons;
    }

    protected async override ValueTask InitializeAsync(CompositeDisposable disposables)
    {
        await GetAll();
    }
}
