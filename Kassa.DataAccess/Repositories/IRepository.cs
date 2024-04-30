using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Kassa.DataAccess.Model;

namespace Kassa.DataAccess.Repositories;
public interface IRepository<T> where T : class, IModel
{
    public Task<T?> Get(Guid id);
    public Task<IEnumerable<T>> GetAll();
    public Task Add(T item);
    public Task Update(T item);
    public Task Delete(T item);
    public Task DeleteAll();

    internal static IRepository<T> CreateMock(string jsonResourceName)
    {
        var assembly = typeof(IRepository<>).Assembly;
        var json = assembly.GetManifestResourceStream($"Kassa.DataAccess.{jsonResourceName}");
        var items = JsonSerializer.Deserialize<IEnumerable<T>>(json!);
        return new MockRepository(items?.ToDictionary(x => x.Id) ?? []);
    }

    internal class MockRepository(Dictionary<Guid, T> items) : IRepository<T>
    {
        public readonly Dictionary<Guid, T> _items = items;

        public Task<T?> Get(Guid categoryId) => Task.FromResult(items.TryGetValue(categoryId, out var value) ? value : null);

        public Task<IEnumerable<T>> GetAll() => Task.FromResult(items.Values.AsEnumerable());

        public Task Add(T item)
        {
            items.Add(item.Id, item);
            return Task.CompletedTask;
        }

        public Task Update(T item)
        {
            items[item.Id] = item;
            return Task.CompletedTask;
        }

        public Task Delete(T item)
        {
            items.Remove(item.Id);
            return Task.CompletedTask;
        }

        public Task DeleteAll()
        {
            items.Clear();
            return Task.CompletedTask;
        }
    }

    internal abstract class BasicMockRepository(IRepository<T> repository) : IRepository<T>
    {
        public Task Add(T item) => repository.Add(item);
        public Task Delete(T item) => repository.Delete(item);
        public Task DeleteAll() => repository.DeleteAll();
        public Task<T?> Get(Guid id) => repository.Get(id);
        public Task<IEnumerable<T>> GetAll() => repository.GetAll();
        public Task Update(T item) => repository.Update(item);
    }
}
