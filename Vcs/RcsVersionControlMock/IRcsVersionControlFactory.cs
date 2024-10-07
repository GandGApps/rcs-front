using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RcsVersionControlMock;

public interface IRcsVersionControlFactory
{
    IRcsVersionControl Create(string appPath);
}