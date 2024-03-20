using System;
using System.Collections.Generic;
using System.Text;
using Kassa.DataAccess.Model;

namespace Kassa.DataAccess.Repositories;
public interface IStreetRepository : IRepository<Street>
{
    internal static IStreetRepository CreateStreetMock(string jsonResourceName)
    {
        return new MockStreetRepository(IRepository<Street>.CreateMock(jsonResourceName));
    }

    internal class MockStreetRepository(IRepository<Street> repository) : BasicMockRepository(repository), IStreetRepository
    {
    }
}
