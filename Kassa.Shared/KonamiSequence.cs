using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kassa.Shared;
public sealed class KonamiSequence
{
    private static readonly string[] _sequence = ["Up", "Up", "Down", "Down", "Left", "Right", "Left", "Right", "B", "A"];

    private int _position = 0;

    public bool IsCompletedBy(string input)
    {

        if (input.Equals(_sequence[_position], StringComparison.InvariantCultureIgnoreCase))
        {
            _position++;
            if (_position == _sequence.Length)
            {

                _position = 0;
                return true;
            }
        }
        else
        {

            _position = 0;
        }

        return false;
    }
}
