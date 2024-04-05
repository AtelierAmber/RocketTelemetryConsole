using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RocketTelemetryConsole.Data.Serial
{
  public interface ISerialParser
  {

    public FlightInfo? Parse(string serial);
  }
}
