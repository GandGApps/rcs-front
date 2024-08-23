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
using Splat;

namespace Kassa.DataAccess.HttpRepository;
internal sealed class ContributionReasonRepository : IRepository<ContributionReason>
{
    private FrozenDictionary<Guid, ContributionReason> _contributionReasons = FrozenDictionary<Guid, ContributionReason>.Empty;

    public Task Add(ContributionReason item) => throw new NotImplementedException();
    public Task Delete(ContributionReason item) => throw new NotImplementedException();
    public Task DeleteAll() => throw new NotImplementedException();

    public async Task<ContributionReason?> Get(Guid id)
    {
        if (_contributionReasons.Count == 0)
        {
            await GetAll();
        }

        if (_contributionReasons.TryGetValue(id, out var contributionReason))
        {
            return contributionReason;
        }

        return null;
    }

    public async Task<IEnumerable<ContributionReason>> GetAll()
    {
        var fundApi = Locator.Current.GetRequiredService<IFundApi>();

        var reasons = await fundApi.GetContributions();

        var contributionReasons = reasons.Select(x => new ContributionReason()
        {
            Id = x.Id,
            Name = x.Name ?? string.Empty,
            IsRequiredComment = x.IsRequiredComment ?? false
        }).ToArray();

        _contributionReasons = contributionReasons.ToFrozenDictionary(x => x.Id);

        return contributionReasons;
    }

    public Task Update(ContributionReason item) => throw new NotImplementedException();
}