using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;

namespace Kassa.RxUI;
public class DocumentRowViewModel : ReactiveObject
{
    public int Number
    {
        get; set;
    }

    public string TypeOfDocument
    {
        get; set;
    }

    public string Time
    {
        get; set;
    }

    public string AuthorizedBy
    {
        get; set;
    }

    public string Composition
    {
        get; set;
    }
}
