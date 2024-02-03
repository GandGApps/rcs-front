using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using DynamicData;
using Kassa.BuisnessLogic.Dto;
using Kassa.DataAccess;

namespace Kassa.BuisnessLogic.Services;
public class AdditiveService(IAdditiveRepository repository) : IAdditiveService, IRuntimeDtoProvider<AdditiveDto, int, Additive>
{
    public SourceCache<AdditiveDto, int> RuntimeAdditives
    {
        get;
    } = new(x => x.Id);

    public bool IsInitialized
    {
        get; set;
    }
    public bool IsDisposed
    {
        get; set;
    }
    SourceCache<AdditiveDto, int> IRuntimeDtoProvider<AdditiveDto, int, Additive>.RuntimeDtos => RuntimeAdditives;
    IRepository<Additive> IRuntimeDtoProvider<AdditiveDto, int, Additive>.Repository => repository;


    public async Task DecreaseAddtiveCount(AdditiveDto additiveDto)
    {
        this.ThrowIfNotInitialized();
        ArgumentNullException.ThrowIfNull(additiveDto);

        if (additiveDto.Count <= 0)
        {
            throw new ArgumentException("Additive count must be greater than zero.", nameof(additiveDto));
        }

        await UpdateAdditive(additiveDto);

        RuntimeAdditives.AddOrUpdate(additiveDto with
        {
            Count = additiveDto.Count - 1
        });
    }
    public void Dispose()
    {
        RuntimeAdditives.Dispose();
        IsDisposed = true;
    }

    public ValueTask DisposeAsync()
    {
        Dispose();

        return ValueTask.CompletedTask;
    }

    public async ValueTask<AdditiveDto?> GetAdditiveById(int additiveId)
    {
        this.ThrowIfNotInitialized();

        return await this.GetDtoAndUpdateRuntime(additiveId);
    }

    public async ValueTask<IEnumerable<AdditiveDto>> GetAdditivesByProductId(int id)
    {
        this.ThrowIfNotInitialized();

        var additives = await repository.GetAdditivesByProductId(id);
        var additivesDto = additives.Select(x => AdditiveDto.FromModel(x)!);

        RuntimeAdditives.AddOrUpdate(additivesDto);

        return additivesDto;
    }

    public async Task IncreaseAdditiveCount(AdditiveDto additiveDto)
    {
        this.ThrowIfNotInitialized();

        var foundedAdditive = await repository.Get(additiveDto.Id);

        if (foundedAdditive is null)
        {
            throw new InvalidOperationException($"Additive with id {additiveDto.Id} not found");
        }

        additiveDto = AdditiveDto.FromModel(foundedAdditive with
        {
            Count = foundedAdditive.Count + 1
        });

        await UpdateAdditive(additiveDto);
    }

    public ValueTask Initialize()
    {
        IsInitialized = true;

        return ValueTask.CompletedTask;
    }

    public async Task UpdateAdditive(AdditiveDto additiveDto)
    {
        this.ThrowIfNotInitialized();

        await this.UpdateDto(additiveDto);
    }
}
