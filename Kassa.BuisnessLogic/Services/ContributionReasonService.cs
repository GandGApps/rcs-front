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
internal sealed class ContributionReasonService : BaseInitializableService, IContributionReasonService
{

    private readonly IRepository<ContributionReason> _repository;

    public IApplicationModelManager<ContributionReasonDto> RuntimeContributionReasons
    {
        get;
    }

    public ContributionReasonService(IRepository<ContributionReason> repository)
    {
        RuntimeContributionReasons = new HostModelManager<ContributionReasonDto>();
        _repository = repository;
    }

    protected async override ValueTask InitializeAsync()
    {
        var ContributionReasons = await _repository.GetAll();
        RuntimeContributionReasons.AddOrUpdate(ContributionReasons.Select(Mapper.MapContributionReasonToDto));
    }
}
