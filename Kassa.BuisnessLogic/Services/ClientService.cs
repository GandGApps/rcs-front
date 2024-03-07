using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using DynamicData;
using Kassa.BuisnessLogic.Dto;
using Kassa.DataAccess;

namespace Kassa.BuisnessLogic.Services;
internal class ClientService : BaseInitializableService, IClientService
{
    private readonly IRepository<Client> _repository;

    public ClientService(IRepository<Client> repository)
    {
        _repository = repository;

        RuntimeClients = new(x => x.Id);
    }

    public SourceCache<ClientDto, Guid> RuntimeClients
    {
        get;
    }

    public async Task AddClient(ClientDto client)
    {
        client.Id = Guid.NewGuid();

        var newClient = Mapper.MapDtoToClient(client);

        await _repository.Add(newClient);

        RuntimeClients.AddOrUpdate(client);
    }

    public async Task DeleteClient(Guid id)
    {
        var client = await _repository.Get(id);

        if (client is null)
        {
            throw new InvalidOperationException($"Client with id {id} not found");
        }

        await _repository.Delete(client);
        RuntimeClients.Remove(id);
    }

    public async ValueTask<ClientDto?> GetClientById(Guid id)
    {
        var client = await _repository.Get(id);

        if (client is null)
        {
            return null;
        }

        var clientDto = Mapper.MapClientToDto(client);

        RuntimeClients.AddOrUpdate(clientDto);

        return clientDto;
    }

    public async Task<IEnumerable<ClientDto>> GetClients()
    {
        var clients = (await _repository.GetAll()).Select(Mapper.MapClientToDto);

        RuntimeClients.AddOrUpdate(clients);

        return clients;
    }
    public async Task UpdateClient(ClientDto client)
    {
        var foundedClient = await _repository.Get(client.Id);

        if (foundedClient is null)
        {
            throw new InvalidOperationException($"Client with id {client.Id} not found");
        }

        var updatedClient = Mapper.MapDtoToClient(client);

        await _repository.Update(updatedClient);

        RuntimeClients.AddOrUpdate(client);
    }


    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (disposing)
        {
            RuntimeClients.Dispose();
        }
    }

}
