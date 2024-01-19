using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.DataAccess;
using ReactiveUI;

namespace Kassa.RxUI;
public class CategoryViewModel(Category category) : ReactiveObject
{
    public int Id => category.Id;

    public string Name => category.Name;
}
