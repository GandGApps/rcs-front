using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.BuisnessLogic.Services;

namespace Kassa.Wpf.Services;
internal sealed class RawSerialPort(byte[] rawBytes, string port) : ICashDrawer
{

    public Task Open()
    {
        using var serialPort = new SerialPort(port);

        serialPort.Write(rawBytes, 0, rawBytes.Length);

        return Task.CompletedTask;
    }
}
