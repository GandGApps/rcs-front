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
    private MainViewModel _instance;
    private readonly IAuthService _authService;
    private readonly IShiftService _shiftService;
    private readonly IReportShiftService _reportShiftService;

    public MainViewModelProvider(IAuthService authService, IShiftService shiftService, IReportShiftService reportShiftService)
    {
        _authService = authService;
        _shiftService = shiftService;
        _reportShiftService = reportShiftService;

        _instance = new(authService, shiftService, reportShiftService);
    }


    public MainViewModel MainViewModel
    {
        get => _instance;
        set => _instance = value;
    }
}
