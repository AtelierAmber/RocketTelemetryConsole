using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RocketTelemetryConsole.Data;
using SadConsole;
using SadConsole.Renderers;
using SadRogue.Primitives;

namespace RocketTelemetryConsole.Screens
{
  public class SpeedGraph : GraphScreen
  {
    public SpeedGraph(int width, int height) : base(width, height, 10, 5, 1.0f, 100.0f)
    {
      SpeedData.CreateDummyData();
      speedRecord = SpeedData.SpeedRecord;
    }

    private Tuple<List<float>, List<float>> speedRecord = new(new(), new());

    protected override void RenderData()
    {
      DevelopAndPrintAxisTicks(speedRecord.Item1, speedRecord.Item2);
      RenderPointData(speedRecord.Item1, speedRecord.Item2);
    }

    protected override void RenderTitle()
    {
      Surface.Print(1, 0, " Speed: " + SpeedData.Speed + " ", SadRogue.Primitives.Color.White);
    }
  }
}
