using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kassa.RxUI;
internal sealed class MainViewModelProvider: IMainViewModelProvider
{
    private MainViewModel? _instance;



    public MainViewModel MainViewModel
    {
        get => _instance ??= new MainViewModel(this);
        set => _instance = value;
    }
}
