using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.BuisnessLogic.Services;
using Kassa.Shared;

namespace Kassa.RxUI;
internal sealed class MainViewModelProvider: IMainViewModelProvider
{
    private MainViewModel? _instance;



    public MainViewModel MainViewModel
    {
        get => _instance ??= RcsKassa.CreateAndInject<MainViewModel>();
        set => _instance = value;
    }
}
