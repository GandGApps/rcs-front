using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Splat;

namespace Kassa.Shared;
public interface IDevelopmentDiagnostics: IEnableLogger
{
    /// <summary>
    /// Проверяет, необходимо ли зарегистрировать сервис.
    /// </summary>
    /// <returns>Возвращает true, если сервис нужно зарегистрировать.</returns>
    public ValueTask<bool> CheckService();
}
