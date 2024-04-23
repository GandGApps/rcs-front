using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicData;
using Kassa.BuisnessLogic.ApplicationModelManagers;
using Kassa.BuisnessLogic.Dto;

namespace Kassa.BuisnessLogic.Services;
public interface IUserService: IInitializableService
{
    public IApplicationModelManager<UserDto> RuntimeUsers
    {
        get;
    }

    public ValueTask<UserDto?> GetUserById(Guid id);
    public Task UpdateUser(UserDto user);
    public Task AddUser(UserDto user);
    public Task DeleteUser(Guid id);
    public Task<IEnumerable<UserDto>> GetUsers();
}
