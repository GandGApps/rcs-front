namespace Kassa.BuisnessLogic.Dto;

public enum WithdrawalErrorId
{
    None = 0,

    /// <summary>
    /// Error occurs when the withdrawal amount is invalid or exceeds the available balance.
    /// </summary>
    AmountError = 1 << 0,

    /// <summary>
    /// Error occurs when the employee does not have sufficient permissions to perform the withdrawal.
    /// </summary>
    AccessError = 1 << 1,

    /// <summary>
    /// An unspecified error occurred during the withdrawal process, 
    /// and a <see cref="ResultWithdrawal.ErrorMessage"/> will be received from the server.
    /// </summary>
    AnotherError = 1 << 2,
}