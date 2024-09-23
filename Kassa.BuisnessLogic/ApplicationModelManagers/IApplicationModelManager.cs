using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.Shared;

namespace Kassa.BuisnessLogic.ApplicationModelManagers;
public interface IApplicationModelManager<TModel> : IReadOnlyDictionary<Guid, TModel>, IObservable<ChangeSet<TModel>>, IDisposable
    where TModel : class, IGuidId
{
    public void AddOrUpdate(TModel model);
    public void AddOrUpdate(IEnumerable<TModel> models);
    public void Remove(Guid id);
    public void Remove(IEnumerable<Guid> ids);
    public void Clear();
    public IDisposable AddPresenter(IApplicationModelPresenter<TModel> presenter);
}
