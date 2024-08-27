using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.BuisnessLogic.ApplicationModelManagers;
using Kassa.BuisnessLogic.Dto;
using Kassa.BuisnessLogic.Services;
using Kassa.DataAccess.Model;
using Kassa.DataAccess.Repositories;

namespace Kassa.BuisnessLogic.Edgar.Services;
internal sealed class SeizureReasonService : BaseInitializableService, ISeizureReasonService
{
    private readonly IRepository<SeizureReason> _repository;

    public IApplicationModelManager<SeizureReasonDto> RuntimeSeizureReasons
    {
        get;
    }

    public SeizureReasonService(IRepository<SeizureReason> repository)
    {
        RuntimeSeizureReasons = new HostModelManager<SeizureReasonDto>();
        _repository = repository;
    }

    protected async override ValueTask InitializeAsync()
    {
        var seizureReasons = await _repository.GetAll();
        RuntimeSeizureReasons.AddOrUpdate(seizureReasons.Select(Mapper.MapSeizureReasonToDto));
    }
}
