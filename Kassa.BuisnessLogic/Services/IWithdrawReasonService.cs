using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.BuisnessLogic.ApplicationModelManagers;
using Kassa.BuisnessLogic.Dto;

namespace Kassa.BuisnessLogic.Services;
public interface ISeizureReasonService: IInitializableService
{
    public IApplicationModelManager<SeizureReasonDto> RuntimeSeizureReasons
    {
        get;
    }
}
