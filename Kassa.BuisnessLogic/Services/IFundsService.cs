using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.BuisnessLogic.Dto;

namespace Kassa.BuisnessLogic.Services;

/// <summary>
/// This service is responsible for handling the withdrawal and deposit of funds.
/// It provides methods to withdraw funds with a specified reason and optionally link it to a member, 
/// as well as to deposit funds with a specified reason and member.
/// </summary>
public interface IFundsService
{
    public Task<ResultWithdrawal> Withdrawal(double amount, Guid? memberId, WithdrawalReasonDto withdrawalReason);
    public Task<DepositActDto> Deposit(double amount, Guid? memberId, DepositReasonDto depositReason);
}
