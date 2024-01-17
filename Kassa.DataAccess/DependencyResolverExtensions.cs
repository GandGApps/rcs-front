using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Splat;

namespace Kassa.DataAccess;
public static class DependencyResolverExtensions
{
    public static void RegisterMockDataAccess(this IMutableDependencyResolver services)
    {
        services.RegisterConstant(IRepository<Product>.CreateMock("MockProducts.json"));
        services.RegisterConstant(IRepository<Category>.CreateMock("MockCategories.json"));
    }
}
