using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kassa.DataAccess.Model;
public enum PaymentType
{
    /// <summary>
    /// Наличные
    /// </summary>
    Cash,
    /// <summary>
    /// Банковская карта
    /// </summary>
    BankCard,
    /// <summary>
    /// Безналичный расчет
    /// </summary>
    CashlessPayment,
    /// <summary>
    /// Без выручки
    /// </summary>
    WithoutRevenue
}
