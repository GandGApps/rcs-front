using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.DataAccess.HttpRepository.Api;
using Kassa.DataAccess.Model;
using Kassa.DataAccess.Repositories;
using Kassa.Shared;
using Kassa.Shared.ServiceLocator;
using Splat;

namespace Kassa.DataAccess.HttpRepository;
internal sealed class ContributionReasonRepository : IRepository<ContributionReason>, IEnableLogger
{
    private readonly FrozenMemoryCache<ContributionReason> _cahce = new();
    private readonly IFundApi _api;
    public ContributionReasonRepository(IFundApi api)
    {
        _api = api;
    }

    public Task Add(ContributionReason item) => throw new NotImplementedException();
    public Task Delete(ContributionReason item) => throw new NotImplementedException();
    public Task DeleteAll() => throw new NotImplementedException();

    public async Task<ContributionReason?> Get(Guid id)
    {
        if (_cahce.IsEmpty)
        {
            await GetAll();

            if (_cahce.IsEmpty)
            {
                this.Log().Error("ContributionReasons is null");
                return null;
            }
        }

        if (_cahce.TryGetValue(id, out var contributionReason))
        {
            return contributionReason;
        }

        return null;
    }

    public async Task<IEnumerable<ContributionReason>> GetAll()
    {
        var reasons = await _api.GetContributions();

        var contributionReasons = reasons.Select(x => new ContributionReason()
        {
            Id = x.Id,
            Name = x.Name ?? string.Empty,
            IsRequiredComment = x.IsRequiredComment is true
        }).ToArray();

        _cahce.Refresh(contributionReasons);

        return contributionReasons;
    }

    public Task Update(ContributionReason item) => throw new NotImplementedException();
}