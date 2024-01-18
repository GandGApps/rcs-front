using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;

namespace Kassa.RxUI;
public interface IInitializableViewModel
{
    ValueTask InitializeAsync();
}
