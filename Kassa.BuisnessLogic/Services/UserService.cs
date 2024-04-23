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
internal class UserService : BaseInitializableService, IUserService
{
    private readonly HostModelManager<UserDto> _runtimeUsers = new();
    private readonly IRepository<User> _repository;

    public UserService(IRepository<User> repository)
    {
        _repository = repository;
        _runtimeUsers.DisposeWith(InternalDisposables);
    }

    public IApplicationModelManager<UserDto> RuntimeUsers => _runtimeUsers;

    public async ValueTask<UserDto?> GetUserById(Guid id)
    {
        var user = await _repository.Get(id);

        if (user is null)
        {
            return null;
        }

        var userDto = Mapper.MapUserToDto(user);

        RuntimeUsers.AddOrUpdate(userDto);

        return userDto;
    }

    public async Task UpdateUser(UserDto user)
    {
        var userToUpdate = await _repository.Get(user.Id);

        if (userToUpdate is null)
        {
            throw new InvalidOperationException($"User with id {user.Id} not found");
        }

        userToUpdate = Mapper.MapDtoToUser(user);

        await _repository.Update(userToUpdate);

        RuntimeUsers.AddOrUpdate(user);
    }

    public async Task AddUser(UserDto user)
    {
        user.Id = Guid.NewGuid();

        var newUser = Mapper.MapDtoToUser(user);

        await _repository.Add(newUser);

        RuntimeUsers.AddOrUpdate(user);
    }

    public async Task DeleteUser(Guid id)
    {
        var user = await _repository.Get(id);

        if (user is null)
        {

            throw new InvalidOperationException($"User with id {id} not found");
        }

        await _repository.Delete(user);

        RuntimeUsers.Remove(id);
    }

    public async Task<IEnumerable<UserDto>> GetUsers()
    {
        var users = (await _repository.GetAll()).Select(Mapper.MapUserToDto);

        RuntimeUsers.AddOrUpdate(users);

        return users;
    }
}
