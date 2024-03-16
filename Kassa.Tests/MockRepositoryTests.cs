using Kassa.DataAccess.Model;
using Kassa.DataAccess.Repositories;

namespace Kassa.Tests;

[TestClass]
public class MockTests
{
    [TestMethod]
    public async Task MockRepositoryTests()
    {
        var repository = IRepository<Category>.CreateMock("MockCategories.json");
        var categories = await repository.GetAll();
        var firstGuid = Guid.Parse("01BCEF7E-4F00-406C-87DA-6364B4900940");

        Assert.AreEqual(firstGuid, categories.First(x => x.Id == firstGuid).Id);
    }

    [TestMethod]
    public async Task MockRepositoryTests2()
    {
        var repository = IRepository<Product>.CreateMock("MockProducts.json");
        var products = await repository.GetAll();
        var firstGuid = Guid.Parse("0011C9AB-9502-4687-B32D-9E6ACC752B6C");

        Assert.AreEqual(firstGuid, products.First(x => x.Id == firstGuid).Id);
    }

    [TestMethod]
    public async Task MockRepositoryTests3()
    {
        var repository = IRepository<Additive>.CreateMock("MockAdditive.json");
        var receipts = await repository.GetAll();
        var firstGuid = Guid.Parse("0011C9AB-9502-4687-B32D-9E6ACC752B1C");

        Assert.AreEqual(firstGuid, receipts.First(x => x.Id == firstGuid).Id);
    }

    [TestMethod]
    public async Task MockRepositoryTests4()
    {
        var repository = IRepository<Client>.CreateMock("MockClient.json");
        var clients = await repository.GetAll();
        var firstGuid = Guid.Parse("7f679106-42e4-43aa-84f6-307c22b1ebc5");

        Assert.AreEqual(firstGuid, clients.First(x => x.Id == firstGuid).Id);
    }
}