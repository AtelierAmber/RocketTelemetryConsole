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

    private static Tuple<List<float>, List<float>> altitudeRecords = new(new(), new());

    public static event EventHandler<Tuple<List<float>, List<float>>>? OnAltitudeRecordChanged;

    public static void PublishRecord(float tminus, float altitude)
    {
      currentAltitude = altitude;
      altitudeRecords.Item1.Add(tminus);
      altitudeRecords.Item2.Add(altitude);

      OnAltitudeRecordChanged?.Invoke(null, altitudeRecords);
    }

    public static Tuple<List<float>, List<float>> GetFullRecord()
    {
      return altitudeRecords;
    }
  }
}
