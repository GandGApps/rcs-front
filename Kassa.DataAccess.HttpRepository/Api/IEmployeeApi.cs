using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.DataAccess.Model;
using Refit;

namespace Kassa.DataAccess.HttpRepository.Api;
internal interface IEmployeeApi
{
    [Get("/employee")]
    public Task<IEnumerable<Member>> GetMembers();

    [Get("/employee/single")]
    public Task<Member> GetMember([AliasAs("user_id")] Guid id);
}
