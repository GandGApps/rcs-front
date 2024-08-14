using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kassa.BuisnessLogic.Dto;
public sealed class ResultWithdrawal
{
    public static readonly ResultWithdrawal DefaultAmountError = Error(WithdrawalErrorId.AmountError, string.Empty);
    public static readonly ResultWithdrawal DefaultAccessError = Error(WithdrawalErrorId.AccessError, string.Empty);

    public static ResultWithdrawal Error(WithdrawalErrorId withdrawalErrorId, string errorMessage) => new(null, errorMessage, false, withdrawalErrorId);
    public static ResultWithdrawal Success(WithdrawalActDto withdrawalAct) => new(withdrawalAct);

    private ResultWithdrawal(WithdrawalActDto withdrawalAct) : this(withdrawalAct, null, true, WithdrawalErrorId.None)
    {
    }

    private ResultWithdrawal(WithdrawalActDto? withdrawalAct, string? errorMessage, bool isSuccess, WithdrawalErrorId withdrawalErrorId)
    {
        WithdrawalAct = withdrawalAct;
        IsSuccess = isSuccess;
        ErrorMessage = errorMessage;
        ErrorId = withdrawalErrorId;
    }



    [MemberNotNullWhen(true, nameof(IsSuccess))]
    public WithdrawalActDto? WithdrawalAct
    {
        get;
    }

    public bool IsSuccess
    {
        get;
    }

    /// <summary>
    /// If <see cref="ErrorMessage"/> is <see cref="string.Empty"/>, a default message should be used. 
    /// However, this still indicates that an error occurred.
    /// </summary>
    [MemberNotNullWhen(false, nameof(IsSuccess))]
    public string? ErrorMessage
    {
        get; 
    }

    public WithdrawalErrorId ErrorId
    {
        get;
    }
}