using RocketTelemetryConsole.Data;
using SadConsole;

namespace RocketTelemetryConsole.Screens
{
  public class AltitudeGraph : GraphScreen
  {
    public AltitudeGraph(int width, int height) : base(width, height, 10, 8, 1.0f, 500.0f)
    {
      altitudeRecord = AltitudeData.GetFullRecord();

      AltitudeData.OnAltitudeRecordChanged += UpdateAltitudeData;
    }

    private Tuple<List<float>, List<float>> altitudeRecord = new(new(), new());

    private void UpdateAltitudeData(object? sender, Tuple<List<float>, List<float>> records)
    {
      altitudeRecord = records;
      hasUpdate = true;
    }

    protected override void RenderData()
    {
      DevelopAndPrintAxisTicks(altitudeRecord.Item1, altitudeRecord.Item2);
      RenderPointData(altitudeRecord.Item1, altitudeRecord.Item2);
    }

    protected override void RenderTitle()
    {
      Surface.Print(1, 0, " Altitude: " + AltitudeData.Altitude + " ", SadRogue.Primitives.Color.White);
    }
  }
}