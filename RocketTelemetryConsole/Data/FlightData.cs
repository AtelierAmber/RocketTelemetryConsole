using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RocketTelemetryConsole.Data
{
  public class FlightData
  {
    public static DateTime LaunchTime { get; private set; }

    public static void SetLaunch(DateTime time)
    {
      LaunchTime = time;
    }
  }
}
