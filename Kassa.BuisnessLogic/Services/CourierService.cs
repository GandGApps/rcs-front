using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;
using Kassa.BuisnessLogic.ApplicationModelManagers;
using Kassa.BuisnessLogic.Dto;
using Kassa.DataAccess.Model;
using Kassa.DataAccess.Repositories;

namespace Kassa.BuisnessLogic.Services;
internal class CourierService(IRepository<Courier> repository) : BaseInitializableService, ICourierService
{
    public IApplicationModelManager<CourierDto> RuntimeCouriers
    {
        get;
    } = new HostModelManager<CourierDto>();

    public async Task AddClient(CourierDto client)
    {
        await repository.Add(Mapper.MapDtoToCourier(client));

        RuntimeCouriers.AddOrUpdate(client);
    }
    public async Task DeleteCourier(Guid id)
    {
        var courier = await repository.Get(id);

        if (courier == null)
        {
            return;
        }

        await repository.Delete(courier);
        RuntimeCouriers.Remove(id);
    }
    public async ValueTask<CourierDto?> GetCourierById(Guid id)
    {
        var courier = await repository.Get(id);

        if (courier == null)
        {
            return null;
        }

        var dto = Mapper.MapCourierToDto(courier);

        RuntimeCouriers.AddOrUpdate(dto);

        return dto;
    }
    public async Task<IEnumerable<CourierDto>> GetCouriers()
    {
        var couriers = await repository.GetAll();

        var dtos = couriers.Select(Mapper.MapCourierToDto);

        RuntimeCouriers.AddOrUpdate(dtos);

        return dtos;
    }

    public async Task UpdateCourier(CourierDto courier)
    {
        var model = await repository.Get(courier.Id);

        if (model == null)
        {
            throw new InvalidOperationException($"Courier with id {courier.Id} not found");
        }

        await repository.Update(Mapper.MapDtoToCourier(courier));

        RuntimeCouriers.AddOrUpdate(courier);
    }

    protected async override ValueTask InitializeAsync(CompositeDisposable disposables)
    {
        var allCouriers = await repository.GetAll();

        var dtos = allCouriers.Select(Mapper.MapCourierToDto);

        RuntimeCouriers.AddOrUpdate(dtos);
    }
}
