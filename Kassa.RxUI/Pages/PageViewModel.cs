﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;

namespace Kassa.RxUI.Pages;
public class PageViewModel : BaseViewModel, IRoutableViewModel
{
    public PageViewModel(MainViewModel mainViewModel) : base(mainViewModel)
    {
        HostScreen = mainViewModel;
    }

    public string? UrlPathSegment
    {
        get;
    }

    public IScreen HostScreen
    {
        get;
    }
}
