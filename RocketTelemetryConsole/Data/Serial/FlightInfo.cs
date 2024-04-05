using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RocketTelemetryConsole.Data.Serial
{
  public struct FlightInfo
  {
    public float TMinus;
    public float? Altitude;
    public float? Speed;
    public float[]? Direction = new float[3];
    public float? Temperature;
    public float[]? GPS = new float[2];

    public string[]? Reports;

    public FlightInfo(float tMinus) { TMinus = tMinus; }
  }
}
