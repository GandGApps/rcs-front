using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kassa.Shared;

/// <summary>
/// Implement this interface to dispatch actions to the UI thread.
/// </summary>
/// <remarks>
/// Implement this interface in the Presentation layer.
/// </remarks>
public interface IDispatcher
{
    public void InvokeAsync(Action action);
    public void InvokeAsync(Func<Task> action);
}
