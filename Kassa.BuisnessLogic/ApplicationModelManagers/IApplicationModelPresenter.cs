using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.DataAccess.Model;

namespace Kassa.BuisnessLogic.ApplicationModelManagers;
public interface IApplicationModelPresenter<TModel>: IDisposable
    where TModel : class, IModel
{
    public Guid Id
    {
        get;
    }

    public void ModelChanged(Change<TModel> change);
}
