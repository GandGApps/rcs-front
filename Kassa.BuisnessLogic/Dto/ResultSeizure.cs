using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kassa.BuisnessLogic.Dto;
public sealed class ResultSeizure
{
    public static readonly ResultSeizure DefaultAmountError = Error(SeizureErrorId.AmountError, string.Empty);
    public static readonly ResultSeizure DefaultAccessError = Error(SeizureErrorId.AccessError, string.Empty);

    public static ResultSeizure Error(SeizureErrorId withdrawalErrorId, string errorMessage) => new(null, errorMessage, false, withdrawalErrorId);
    public static ResultSeizure Success(SeizureActDto withdrawalAct) => new(withdrawalAct);

    private ResultSeizure(SeizureActDto withdrawalAct) : this(withdrawalAct, null, true, SeizureErrorId.None)
    {
    }

    private ResultSeizure(SeizureActDto? withdrawalAct, string? errorMessage, bool isSuccess, SeizureErrorId withdrawalErrorId)
    {
        WithdrawalAct = withdrawalAct;
        IsSuccess = isSuccess;
        ErrorMessage = errorMessage;
        ErrorId = withdrawalErrorId;
    }



    [MemberNotNullWhen(true, nameof(IsSuccess))]
    public SeizureActDto? WithdrawalAct
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

    public SeizureErrorId ErrorId
    {
        get;
    }
}