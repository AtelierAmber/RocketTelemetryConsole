using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RocketTelemetryConsole.Data
{
  public static class AltitudeData
  {
    public static float Altitude { get => currentAltitude; private set => currentAltitude = value; }
    private static float currentAltitude = 0;

    public static Tuple<List<int>, List<float>> GetRecord()
    {
      Tuple<List<int>, List<float>> data = new(new(),new());
      data.Item1.Add(0);
      data.Item2.Add(5000);
      data.Item1.Add(1);
      data.Item2.Add(6000);
      data.Item1.Add(2);
      data.Item2.Add(7000);
      data.Item1.Add(3);
      data.Item2.Add(8000);
      data.Item1.Add(4);
      data.Item2.Add(9000);
      data.Item1.Add(5);
      data.Item2.Add(8000);
      data.Item1.Add(6);
      data.Item2.Add(6500);
      data.Item1.Add(7);
      data.Item2.Add(6000);
      data.Item1.Add(8);
      data.Item2.Add(5500);
      data.Item1.Add(9);
      data.Item2.Add(5000);
      return data;
    }
  }
}
