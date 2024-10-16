using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Kassa.RxUI;
public static class MainViewModelProviderServiceExtensions
{
    public static void AddMainViewModelProvider(this IServiceCollection services)
    {
        services.AddSingleton<IMainViewModelProvider,MainViewModelProvider>();
        services.AddSingleton<MainViewModel>(sp => sp.GetRequiredService<IMainViewModelProvider>().MainViewModel);
    }
}
