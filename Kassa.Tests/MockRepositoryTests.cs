using Kassa.DataAccess;

namespace Kassa.Tests;

[TestClass]
public class MockTests
{
    [TestMethod]
    public async Task MockRepositoryTests()
    {
        var repository = IRepository<Category>.CreateMock("MockCategories.json");
        var categories = await repository.GetAll();

        Assert.AreEqual(1, categories.First(x => x.Id == 1).Id);
    }

    [TestMethod]
    public async Task MockRepositoryTests2()
    {
        var repository = IRepository<Product>.CreateMock("MockProducts.json");
        var products = await repository.GetAll();

        Assert.AreEqual(1, products.First(x => x.Id == 1).Id);
    }

    [TestMethod]
    public async Task MockRepositoryTests3()
    {
        var repository = IRepository<Additive>.CreateMock("MockAdditive.json");
        var receipts = await repository.GetAll();

        Assert.AreEqual(1, receipts.First(x => x.Id == 1).Id);
    }
}