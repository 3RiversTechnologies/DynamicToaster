using System;
using System.IO.Ports;
using System.Text;
using System.Threading;

public class PortChat : IDisposable
{
    private string _portName;
    private SerialPort _port;

    public void Dispose()
    {
        if (_port != null)
        {
            if (_port.IsOpen)
            {
                _port.Close();
            }
            _port = null;
        }
    }

    private void WriteCommand(string command)
    {
        var commandBuffer = Encoding.ASCII.GetBytes(command + '\n');
        _port.Write(commandBuffer, 0, commandBuffer.Length);
    }

    private string GetResponse()
    {
        var line1 = _port.ReadLine();
        var line2 = _port.ReadLine();
        return line2;
    }

    private bool IsOurDevice(string portName)
    {
        _port = new SerialPort();
        try
        {

            // Allow the user to set the appropriate properties.
            _port.PortName = portName;
            _port.BaudRate = 115200;
            _port.Parity = Parity.None;
            _port.DataBits = 8;
            _port.StopBits = StopBits.One;
            _port.Handshake = Handshake.None;

            _port.ReadTimeout = 500;
            _port.WriteTimeout = 500;

            _port.Open();

            WriteCommand("ATI");

            var deviceName = GetResponse();
            if (deviceName.StartsWith("DYNA_RELAY"))
            {
                return true;
            }
        }
        catch (Exception e)
        {
        }
        _port.Close();
        return false;
    }

    public bool FindPort()
    {
        string[] ports = SerialPort.GetPortNames();

        Console.WriteLine("The following serial ports were found:");

        // Display each port name to the console.
        foreach (string port in ports)
        {
            Console.WriteLine(port);
        }
        foreach (string s in ports)
        {
            if (IsOurDevice(s))
            {
                return true;
            }
        }
        return false;
    }

    public void SendMessage(string unknown)
    {
        WriteCommand("SOMETHING HERE");
        //GetResponse();
    }
}

