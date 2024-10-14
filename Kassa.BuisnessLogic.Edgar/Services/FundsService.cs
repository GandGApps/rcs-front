using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;
using Kassa.BuisnessLogic.Dto;
using Kassa.BuisnessLogic.Edgar.Api;
using Kassa.BuisnessLogic.Services;
using Kassa.DataAccess.Model;
using Kassa.Shared;
using Kassa.Shared.ServiceLocator;

namespace Kassa.BuisnessLogic.Edgar.Services;
internal sealed class FundsService: BaseInitializableService, IFundsService
{
    private readonly IFundApi _fundApi;
    private readonly IShiftService _shiftService;

    public FundsService(IFundApi fundApi, IShiftService shiftService)
    {
        _fundApi = fundApi;
        _shiftService = shiftService;
    }

    private double CashBalanceValue
    {
        get => _cashBalance.Value;
        set => _cashBalance.OnNext(value);
    }
    private readonly AdapterBehaviorSubject<double> _cashBalance = new(0);

    /// <inheritdoc/>
    public IObservableOnlyBehaviourSubject<double> CashBalance => _cashBalance;
    
    public async Task<ContributionActDto> Contribute(double amount, string comment, Guid memberId, string pincode, ContributionReasonDto contributionReason)
    {
        var cashierShiftId = _shiftService.CurrentCashierShift.Value!.CreateDto().Id;

        var contributeRequest = new ContributeRequest(cashierShiftId, comment, amount, memberId, pincode, contributionReason.Id);

        await _fundApi.Contribute(contributeRequest);

        CashBalanceValue += amount;

        return null;
    }

    public async Task<ResultSeizure> Seize(double amount, string comment, Guid memberId, string pincode, SeizureReasonDto seizureReason)
    {
        var cashierShiftId = _shiftService.CurrentCashierShift.Value!.CreateDto().Id;

        var contributeRequest = new SeizureRequest(cashierShiftId, comment, amount, memberId, pincode, seizureReason.Id);

        await _fundApi.Seizure(contributeRequest);

        CashBalanceValue -= amount;

        return null;
    }

    public async Task GetCashBalance()
    {
        var cashierShiftId = _shiftService.CurrentCashierShift.Value!.CreateDto().Id;

        var fund = await _fundApi.GetFunds(cashierShiftId);

        CashBalanceValue = fund.Funds ?? 0;
    }

    protected async override ValueTask InitializeAsync(CompositeDisposable disposables)
    {
        await GetCashBalance();
    }
}
