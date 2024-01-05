using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kassa.RxUI.Dialogs;
public class HintVm(string hint, object target, object? root = null)
{
    public string Hint
    {
        get; set;
    } = hint ?? throw new ArgumentNullException(nameof(hint));

    public object Target
    {
        get; set;
    } = target ?? throw new ArgumentNullException(nameof(target));

    public object? Root
    {
        get; set;
    } = root;
}
