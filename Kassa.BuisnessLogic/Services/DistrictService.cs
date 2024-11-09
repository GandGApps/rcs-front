using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Diagnostics;
using DynamicData;
using Kassa.BuisnessLogic.Dto;
using Kassa.DataAccess.Model;
using Kassa.DataAccess.Repositories;

namespace Kassa.BuisnessLogic.Services;
internal class DistrictService : BaseInitializableService, IDistrictService
{
    private readonly IDistrictRepository _repository;
    public DistrictService(IDistrictRepository repository)
    {
        _repository = repository;

        RuntimeDistricts = new(x => x.Id);
    }

    public SourceCache<DistrictDto, Guid> RuntimeDistricts
    {
        get;
    }

    protected async override ValueTask InitializeAsync(CompositeDisposable disposables)
    {
        var districts = await _repository.GetAll();

        var dtos = districts.Select(Mapper.MapDistrictToDto);

        RuntimeDistricts.AddOrUpdate(dtos);
    }

    public async ValueTask AddDistrict(DistrictDto district)
    {

        district.Id = Guid.NewGuid();

        var newDistrict = Mapper.MapDtoToDistrict(district);

        await _repository.Add(newDistrict);

        RuntimeDistricts.AddOrUpdate(district);
    }

    public async ValueTask DeleteDistrict(Guid id)
    {
        var district = await _repository.Get(id) ?? ThrowHelper.ThrowArgumentException<District>($"District with id {id} not found");
        await _repository.Delete(district);
        RuntimeDistricts.Remove(id);
    }

    public async ValueTask<IEnumerable<DistrictDto>> GetDistricts()
    {
        var districts = await _repository.GetAll();

        var dtos = districts.Select(Mapper.MapDistrictToDto);

        RuntimeDistricts.AddOrUpdate(dtos);

        return dtos;
    }

    public async ValueTask<DistrictDto?> GetDistrictById(Guid id)
    {
        var district = await _repository.Get(id);

        if (district is null)
        {
            return null;
        }

        var districtDto = Mapper.MapDistrictToDto(district);

        RuntimeDistricts.AddOrUpdate(districtDto);

        return districtDto;
    }

    public async ValueTask UpdateDistrict(DistrictDto district)
    {
        var existingDistrict = await _repository.Get(district.Id);

        if (existingDistrict is null)
        {
            throw new InvalidOperationException($"District with id {district.Id} not found");
        }

        var updatedDistrict = Mapper.MapDtoToDistrict(district);

        await _repository.Update(updatedDistrict);

        RuntimeDistricts.AddOrUpdate(district);
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (disposing)
        {
            RuntimeDistricts.Dispose();
        }
    }

    public ValueTask<IEnumerable<StreetDto>> GetStreets(DistrictDto districtDto) => throw new NotImplementedException();
}
