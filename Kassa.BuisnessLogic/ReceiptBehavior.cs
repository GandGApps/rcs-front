using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kassa.BuisnessLogic;
public enum ReceiptBehavior
{
    NoPrintReceipt = 0,
    PrintReceipt = 1 << 0,
    SendToEmail = 1 << 1,
}