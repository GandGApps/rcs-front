using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.BuisnessLogic.Dto;

namespace Kassa.BuisnessLogic.Services;

/// <summary>
/// This service is responsible for handling the seize and contribute of funds.
/// It provides methods to withdraw funds with a specified reason and optionally link it to a member, 
/// as well as to contribute funds with a specified reason and member.
/// </summary>
public interface IFundsService: IInitializableService
{
    /// <summary>
    /// The <see cref="CashBalance"/> property represents the current amount of cash available at the cashier.
    /// </summary>
    public IObservableOnlyBehaviourSubject<double> CashBalance
    {
        get;
    }

    public Task<ResultSeizure> Seize(double amount, string? comment, Guid memberId,string pincode, SeizureReasonDto seizureReason);
    public Task<ContributionActDto> Contribute(double amount, string? comment, Guid memberId, string pincode, ContributionReasonDto contributionReason);
    public Task GetCashBalance();
}
