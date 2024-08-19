using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.BuisnessLogic.ApplicationModelManagers;
using Kassa.BuisnessLogic.Dto;
using Kassa.DataAccess.Model;
using Kassa.DataAccess.Repositories;

namespace Kassa.BuisnessLogic.Services;
internal sealed class DepositReasonService : BaseInitializableService, IDepositReasonService
{

    private readonly IRepository<DepositReason> _repository;

    public IApplicationModelManager<DepositReasonDto> RuntimeDepositReasons
    {
        get;
    }

    public DepositReasonService(IRepository<DepositReason> repository)
    {
        RuntimeDepositReasons = new HostModelManager<DepositReasonDto>();
        _repository = repository;
    }

    protected async override ValueTask InitializeAsync()
    {
        var depositReasons = await _repository.GetAll();
        RuntimeDepositReasons.AddOrUpdate(depositReasons.Select(Mapper.MapDepositReasonToDto));
    }
}
