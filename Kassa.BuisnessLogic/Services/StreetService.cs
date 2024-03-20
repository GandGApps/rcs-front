using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;
using DynamicData;
using Kassa.BuisnessLogic.Dto;
using Kassa.DataAccess.Repositories;

namespace Kassa.BuisnessLogic.Services;
public class StreetService : BaseInitializableService, IStreetService
{
    private readonly IStreetRepository _streetRepository;
    public StreetService(IStreetRepository streetRepository)
    {
        Streets = new SourceCache<StreetDto, Guid>(x => x.Id);
        _streetRepository = streetRepository;
    }

    public SourceCache<StreetDto, Guid> Streets
    {
        get;
    }

    public async ValueTask<IEnumerable<StreetDto>> GetAllStreets()
    {
        var streets = await _streetRepository.GetAll();

        var streetsDto = streets.Select(x => Mapper.MapStreetToDto(x));

        Streets.AddOrUpdate(streetsDto);

        return streetsDto;
    }

    public async ValueTask<StreetDto> GetStreetById(Guid id)
    {
        var street = await _streetRepository.Get(id);

        var dto = Mapper.MapStreetToDto(street);

        Streets.AddOrUpdate(dto);

        return dto;
    }

    protected async override ValueTask InitializeAsync(CompositeDisposable disposables)
    {
        await base.InitializeAsync(disposables);

        var streets = await _streetRepository.GetAll();

        var streetsDto = streets.Select(x => Mapper.MapStreetToDto(x));

        Streets.AddOrUpdate(streetsDto);
    }
}
