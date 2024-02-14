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
  public class SpeedGraph : ScreenSurface
  {
    public SpeedGraph(int width, int height) : base(width, height)
    {
      GraphArea = new Rectangle((6, 2), (Surface.Width - 3, Surface.Height - 3));
      SpeedData.CreateDummyData();
    }

    private Tuple<List<float>, List<float>> speedRecord = new(new(), new());

    public Rectangle GraphArea { get; private set; }
    public Rectangle GraphRanges { get; private set; }

    private bool hasUpdate = true;

    private int MaxXTicks = 10;
    private int MaxYTicks = 5;

    public override void Update(TimeSpan delta)
    {
      base.Update(delta);
      speedRecord = SpeedData.SpeedRecord;
    }

    public override void Render(TimeSpan delta)
    {
      base.Render(delta);

      if (hasUpdate)
      {
        Surface.Clear();
        RenderBorder();
        RenderAxis();
        RenderPointData();
        hasUpdate = false;
      }
    }

    public void RenderBorder()
    {
      Surface.DrawBox(Surface.Area, ShapeParameters.CreateStyledBoxThin(new Color(60, 60, 60)));
      Surface.Print(1, 0, " Speed: " + 0 + " ", Color.White);
    }

    public void RenderAxis()
    {
      Surface.DrawLine(GraphArea.MinExtent, (GraphArea.X, GraphArea.MaxExtentY), 179);
      Surface.DrawLine((GraphArea.X, GraphArea.MaxExtentY), GraphArea.MaxExtent, 196);
      Surface.SetGlyph(GraphArea.X, GraphArea.MaxExtentY, 192);

      var TMinus = speedRecord.Item1;
      var Speeds = speedRecord.Item2;
      float minY = Speeds.Min();
      float maxY = Speeds.Max();

      float minX = TMinus.First();
      float maxX = TMinus.Last();

      GraphRanges = new Rectangle(((int)minX, (int)minY), ((int)maxX, (int)maxY));

      int numXTicks = Math.Min(TMinus.Count, MaxXTicks + 1);
      int XTickSpacing = (GraphArea.Width - 1) / numXTicks;
      int XTickInterval = TMinus.Count / numXTicks;
      for (int t = 0; t < numXTicks; ++t)
      {
        PrintXTick(GraphArea.X + 1 + (t * XTickSpacing), (uint?)TMinus[Math.Min(t * XTickInterval, TMinus.Count - 1)]);
      }

      int numYTicks = Math.Min(Speeds.Count, MaxYTicks + 1);
      int YTickSpacing = (GraphArea.Width - 1) / numYTicks;
      int YTickInterval = Speeds.Count / numYTicks;
      for (int s = 0; s < numXTicks; ++s)
      {
        PrintYTick(GraphArea.MaxExtentY - 1 - (s * YTickSpacing), (uint?)Speeds[Math.Min(s * YTickInterval, Speeds.Count - 1)]);
      }
    }

    private void PrintXTick(int x, uint? tag = null)
    {
      Surface.SetGlyph(x, GraphArea.MaxExtentY, 193);
      if (tag.HasValue)
      {
        String tagStr = tag.Value.ToString();
        int xInset = tagStr.Length / 2;
        Surface.Print(x - xInset, GraphArea.MaxExtentY + 1, tagStr);
      }
    }

    private void PrintYTick(int y, uint? tag = null)
    {
      Surface.SetGlyph(GraphArea.X, y, 195);
      if (tag.HasValue)
      {
        String tagStr = tag.Value.ToString();
        Surface.Print(GraphArea.X - tagStr.Length, y, tagStr);
      }
    }

    private Point MapPointToGraph(float a, float t)
    {
      int x = (int)((float)(GraphArea.X) + (float)(t - GraphRanges.X) *
                    (float)(GraphArea.MaxExtentX - GraphArea.X) /
                    (float)((GraphRanges.MaxExtentX) - GraphRanges.X));
      int y = (int)((float)(GraphArea.MaxExtentY) + (float)(a - GraphRanges.Y) *
                    (float)(GraphArea.Y - GraphArea.MaxExtentY) /
                    (float)((GraphRanges.MaxExtentY) - GraphRanges.Y));

      Point graphPoint = new Point(x, y);
      return graphPoint;
    }

    private void RenderPointData()
    {
      var TMinus = speedRecord.Item1;
      var Speeds = speedRecord.Item2;

      for (int i = 0; i < TMinus.Count; ++i)
      {
        float a = Speeds[i];
        float t = TMinus[i];
        Point point = MapPointToGraph(a, t);

        Surface.SetGlyph(point.X, point.Y, 249);
      }
    }
  }
}
