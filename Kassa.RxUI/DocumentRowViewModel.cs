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

    public required string TypeOfDocument
    {
        get; set;
    }

    public required string Time
    {
        get; set;
    }

    public required string AuthorizedBy
    {
        get; set;
    }

    public required string Composition
    {
        get; set;
    }
}
