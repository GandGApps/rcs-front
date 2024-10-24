﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kassa.BuisnessLogic.Services;
public interface IApplication
{
    public IObservableOnlyBehaviourSubject<bool> IsOffline
    {
        get;
    }


    public bool TryHandleUnhandeledException(Exception ex);


    public void Exit();

}
