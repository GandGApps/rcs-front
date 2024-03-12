using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicData;
using Kassa.BuisnessLogic.Dto;

namespace Kassa.BuisnessLogic.Services;
public interface IClientService: IInitializableService
{
    public SourceCache<ClientDto, Guid> RuntimeClients
    {
        get;
    }
    public ValueTask<ClientDto?> GetClientById(Guid id);
    public Task UpdateClient(ClientDto client);
    public Task AddClient(ClientDto client);
    public Task DeleteClient(Guid id);
    public Task<IEnumerable<ClientDto>> GetClients();
}
