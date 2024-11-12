using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.BuisnessLogic.Services;
using Kassa.Shared;
using Microsoft.Extensions.DependencyInjection;

namespace Kassa.BuisnessLogic;
public static class ApiRcsvcApiServiceCollectionExtension
{
    public static void AddRcsvcApi(this IServiceCollection services)
    {
        services.AddApi<IRcsvcApi>();
    }
}
