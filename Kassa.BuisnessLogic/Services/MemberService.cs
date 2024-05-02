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
internal class MemberService : BaseInitializableService, IMemberService
{
    private readonly HostModelManager<MemberDto> _runtimeMembers = new();
    private readonly IRepository<Member> _repository;

    public MemberService(IRepository<Member> repository)
    {
        _runtimeMembers.DisposeWith(InternalDisposables);
        _repository = repository;
    }

    public IApplicationModelManager<MemberDto> RuntimeMembers => _runtimeMembers;

    public async Task<MemberDto?> GetMember(Guid id)
    {
        var member = await _repository.Get(id);

        if (member is null)
        {
            return null;
        }

        var memberDto = Mapper.MapMemberToDto(member);

        RuntimeMembers.AddOrUpdate(memberDto);

        return memberDto;
    }

    public async Task<IEnumerable<MemberDto>> GetMembers()
    {
        var members = await _repository.GetAll();

        if (members is null)
        {
            return [];
        }

        var memberDtos = members.Select(Mapper.MapMemberToDto).ToList();

        RuntimeMembers.AddOrUpdate(memberDtos);

        return memberDtos;
    }

    public async Task UpdateMember(MemberDto member)
    {
        var memberToUpdate = await _repository.Get(member.Id);

        if (memberToUpdate is null)
        {
            throw new InvalidOperationException($"Member with id {member.Id} not found");
        }

        memberToUpdate = Mapper.MapDtoToMember(member);

        await _repository.Update(memberToUpdate);

        RuntimeMembers.AddOrUpdate(member);
    }

    public async Task AddMember(MemberDto member)
    {
        member.Id = Guid.NewGuid();

        var newMember = Mapper.MapDtoToMember(member);

        await _repository.Add(newMember);

        RuntimeMembers.AddOrUpdate(member);
    }

    public async Task DeleteMember(MemberDto member)
    {
        var memberToDelete = await _repository.Get(member.Id);

        if (memberToDelete is null)
        {
            throw new InvalidOperationException($"Member with id {member.Id} not found");
        }

        await _repository.Delete(memberToDelete);

        RuntimeMembers.Remove(member.Id);
    }

    public async Task DeleteAllMembers()
    {
        await _repository.DeleteAll();

        RuntimeMembers.Clear();
    }
}
