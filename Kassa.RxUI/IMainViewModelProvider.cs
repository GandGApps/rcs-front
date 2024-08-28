using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kassa.RxUI;
public interface IMainViewModelProvider
{
    public MainViewModel MainViewModel
    {
        get; set;
    }
}
