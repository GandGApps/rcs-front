using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.BuisnessLogic.ApplicationModelManagers;
using Kassa.BuisnessLogic.Dto;
using Kassa.DataAccess.Model;

namespace Kassa.BuisnessLogic.Services;
public interface IMemberService : IInitializableService
{
    public IApplicationModelManager<MemberDto> RuntimeMembers
    {
        get;
    }

    public Task<MemberDto?> GetMember(Guid id);
    public Task<IEnumerable<MemberDto>> GetMembers();
    public Task AddMember(MemberDto member);
    public Task UpdateMember(MemberDto member);
    public Task DeleteMember(MemberDto member);
    public Task DeleteAllMembers();
}
