using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.DataAccess.Model;
using Kassa.Shared.DelegatingHandlers;
using Refit;

namespace Kassa.DataAccess.HttpRepository.Api;
public interface IEmployeePostApi: IUseMemberToken
{
    [Get("employee/posts")]
    Task<IEnumerable<Shift>> GetPosts();

    [Get("employee/posts/{id}")]
    Task<Shift> GetPost(Guid id);
}
