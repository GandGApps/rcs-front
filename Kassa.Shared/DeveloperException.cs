using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kassa.Shared;
/// <summary>
/// Эти ошибки точно будут(должны быть) перехвачены и обработаны. 
/// </summary>
public sealed class DeveloperException(string message): Exception(message)
{
}
