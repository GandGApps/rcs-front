using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Diagnostics;

namespace Kassa.RxUI.Dialogs;
public class HintVm(string hint, object target, object? root = null)
{
    public string Hint
    {
        get; set;
    } = hint ?? ThrowHelper.ThrowArgumentNullException<string>(nameof(hint));

    public object Target
    {
        get; set;
    } = target ?? ThrowHelper.ThrowArgumentNullException<string>(nameof(target));

    public object? Root
    {
        get; set;
    } = root;
}
