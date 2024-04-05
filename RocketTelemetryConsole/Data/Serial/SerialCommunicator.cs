using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RocketTelemetryConsole.Data.Serial.Parsers;
using RocketTelemetryConsole.Screens;

namespace RocketTelemetryConsole.Data.Serial
{
  public class SerialCommunicator
  {
    public SerialCommunicator() { }

    protected bool running = false;

    protected SerialPort? serialPort = null;

    // Parse Version, Parser for that version
    protected Dictionary<int, ISerialParser> parsers = new();

    public virtual void Start(int baudRate)
    {
      LogScreen.Logger.Log("Loading Parsers...");
      LogScreen.Logger.Log($"Loaded {LoadParsers()} parsers.");

      LogScreen.Logger.Log("Initializing Serial Communication");
      string[] ports = SerialPort.GetPortNames();
      if (ports.Length > 0)
      {
        LogScreen.Logger.Log(ports.Length + " serial ports found: ");
        foreach (string port in ports)
        {
          LogScreen.Logger.Log($"  {port}");
        }
        LogScreen.Logger.Log("Connecting to first port...");
        serialPort = new(ports[0], baudRate);
        serialPort.Open();
      }
    }

    public void End()
    {
      if (!running)
      {
        return;
      }
      running = false;
      serialPort?.Close();
      LogScreen.Logger.Log("Serial Communication Closed");
    }

    private int LoadParsers()
    {
      parsers.Add(1, new V1Parser());
      return parsers.Count;
    }

    public virtual void Update()
    {
      if (running)
      {
        string serialVal = serialPort!.ReadExisting();
        string[] split = serialVal.Split('*');

        if (int.TryParse(split[0], out int version))
        {
          if (parsers.TryGetValue(version, out ISerialParser? parser))
          {
            FlightInfo? infoN = parser.Parse(split[1]);
            if (!infoN.HasValue) return;
            FlightInfo info = infoN.Value;
              
            if(info.Reports != null)
            {
              LogScreen.Logger.Log($"Reports received at T-{info.TMinus}:");

              foreach(string rep in info.Reports!)
              {
                LogScreen.Logger.Log($"  {rep}");
              }
            }

            if (info.Altitude.HasValue)
            {
              AltitudeData.PublishRecord(info.TMinus, info.Altitude.Value);
            }
            if (info.Speed.HasValue)
            {
              SpeedData.PublishRecord(info.TMinus, info.Speed.Value);
            }
          }
          else
          {
            LogScreen.Logger.Error($"No Serial Parser for version {version} was found!");
          }
        }
        else
        {
          LogScreen.Logger.Error($"Received serial with no version classification! \"{split[1]}\"");
        }
      }
    }
  }
}
