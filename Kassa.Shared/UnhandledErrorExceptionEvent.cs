using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kassa.Shared;
public delegate ValueTask UnhandledErrorExceptionEvent(object? sender, UnhandledErrorExceptionEventArgs args);
