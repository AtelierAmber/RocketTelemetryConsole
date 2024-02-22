using MonoGame.Extended;
using RocketTelemetryConsole.Data;
using SadConsole;
using SadConsole.Input;
using SadConsole.Renderers;
using SadRogue.Primitives;

using MXna = Microsoft.Xna.Framework;

namespace RocketTelemetryConsole.Screens
{
  public class AltitudeGraph : ScreenSurface
  {
    public AltitudeGraph(int width, int height) : base(width, height) 
    {
      GraphArea = new((7, 2), (Surface.Width - 3, Surface.Height - 4));
      altitudeRecord = AltitudeData.RecieveRecord();
      mouseHandler = new AltitudeGraphMouseHandler(this, GraphArea.X, GraphArea.Y);
    }

    private Tuple<List<int>, List<float>> altitudeRecord = new(new(), new());

    public Rectangle GraphArea { get; private set; }
    public float[] XAxis;
    public int[] YAxis;
    public Rectangle GraphRanges { get; private set; }

    private bool hasUpdate = true;

    private int MaxXTicks = 10;
    private int MaxYTicks = 5;
    private float YGranularity = 500.0f;
    private float XGranularity = 1.0f;

    private AltitudeGraphMouseHandler mouseHandler;

    public override void Update(TimeSpan delta)
    {
      base.Update(delta);
      if (AltitudeData.HasUpdate)
      {
        altitudeRecord = AltitudeData.RecieveRecord(true);
      }
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
      Surface.Print(1, 0, " Altitude: " + AltitudeData.Altitude + " ", Color.White);
    }

    public void RenderAxis()
    {
      Surface.DrawLine((GraphArea.X - 1, GraphArea.MaxExtentY + 1), (GraphArea.MaxExtentX, GraphArea.MaxExtentY + 1), 196); // X Axis
      Surface.DrawLine((GraphArea.X - 1, GraphArea.MaxExtentY + 1), (GraphArea.X - 1, GraphArea.Y + 1), 179); // Y Axis
      Surface.SetGlyph(GraphArea.X - 1, GraphArea.MaxExtentY + 1, 192);

      var TMinus = altitudeRecord.Item1;
      var Altitudes = altitudeRecord.Item2;
       
      //TODO: Move this to a subscribable onAltitudeDataUpdated delegate

      float fullXTicksNum = ((TMinus.Last() - TMinus.First()) / XGranularity);
      float xScalar = 2.0f;
      int xDivisorCount = Utils.Math.DivisorCount(fullXTicksNum, xScalar, MaxXTicks);
      int numXTicks = (int)MathF.Ceiling(fullXTicksNum / MathF.Max(1.0f, (xDivisorCount * xScalar)));
      float minX = TMinus.First();
      float maxX = minX + (numXTicks * MathF.Max(1.0f, xDivisorCount * xScalar) * XGranularity);
      XAxis = new float[GraphArea.Width];

      float fullYTicksNum = ((Altitudes.Max() - Altitudes.Min()) / YGranularity);
      float yScalar = 2.0f;
      int yDivisorCount = Utils.Math.DivisorCount(fullYTicksNum, yScalar, MaxYTicks);
      int numYTicks = (int)MathF.Ceiling(fullYTicksNum / MathF.Max(1.0f, (yDivisorCount * yScalar)));
      float minY = ((int)(Altitudes.Min() / YGranularity)) * YGranularity;
      float maxY = minY + (numYTicks * MathF.Max(1.0f, yDivisorCount * yScalar) * YGranularity);
      YAxis = new int[GraphArea.Height];

      GraphRanges = new Rectangle(((int)minX, (int)minY), ((int)maxX, (int)maxY));

      {
        float XTickSpacing = (GraphArea.MaxExtentX - GraphArea.MinExtentX) / (float)numXTicks;
        float XTickInterval = MathF.Max(1.0f, xDivisorCount * xScalar) * XGranularity;
        int prevXTick = (int)minX;
        int prevXLoc = GraphArea.X;
        for (int t = 0; t <= numXTicks; ++t)
        {
          int xTickVal = (int)Math.Min(minX + (t * XTickInterval), maxX);
          int xTickLoc = (int)MathF.Round(GraphArea.X + (t * XTickSpacing));
          PrintXTick(xTickLoc, (uint?)xTickVal);
          if (t != 0)
          {
            float xTickInterinterval = (float)(xTickVal - prevXTick) / (float)(prevXLoc - xTickLoc);
            for (int x = prevXLoc; x <= xTickLoc; ++x)
            {
              XAxis[x - GraphArea.MinExtentX] = MathF.Round(prevXTick - ((x - (prevXLoc)) * xTickInterinterval), 1);
            }
            prevXTick = xTickVal;
            prevXLoc = xTickLoc;
          }
        }
      }

      {
        float YTickSpacing = (GraphArea.MaxExtentY - GraphArea.MinExtentY) / (float)numYTicks; 
        float YTickInterval = MathF.Max(1.0f, yDivisorCount * yScalar) * YGranularity;
        int prevYTick = (int)minY;
        int prevYLoc = GraphArea.MaxExtentY;
        for (int a = 0; a <= numYTicks; ++a)
        {
          int yTickVal = (int)Math.Min(minY + (a * YTickInterval), maxY);
          int yTickLoc = (int)MathF.Round(GraphArea.MaxExtentY - (a * YTickSpacing));
          PrintYTick(yTickLoc, (uint?)yTickVal);
          if (a != 0)
          {
            float yTickInterinterval = (float)(yTickVal - prevYTick) / (float)(prevYLoc - yTickLoc);
            for(int y = prevYLoc; y >= yTickLoc; --y)
            {
              YAxis[y - GraphArea.MinExtentY] = (int)MathF.Round(prevYTick - ((y - (prevYLoc)) * yTickInterinterval));
            }
            prevYTick = yTickVal;
            prevYLoc = yTickLoc;
          }
        }
      }
    }

    private void PrintXTick(int x, uint? tag = null)
    {
      Surface.SetGlyph(x, GraphArea.MaxExtentY + 1, 193);
      if (tag.HasValue)
      {
        string tagStr = tag.Value.ToString();
        int xInset = tagStr.Length / 2;
        Surface.Print(x - xInset, GraphArea.MaxExtentY + 2, tagStr);
      }
    }

    private void PrintYTick(int y, uint? tag = null)
    {
      Surface.SetGlyph(GraphArea.X - 1, y, 195);
      if (tag.HasValue)
      {
        string tagStr = tag.Value.ToString();
        Surface.Print(GraphArea.X - tagStr.Length - 1, y, tagStr);
      }
    }

    private Point MapPointToGraph(float a, float t)
    {
      float minXFrom = GraphRanges.X;
      float maxXFrom = GraphRanges.MaxExtentX;
      float minXTo = GraphArea.X;
      float maxXTo = GraphArea.MaxExtentX;

      int x = (int)MathF.Round((((t - minXFrom) / (maxXFrom - minXFrom)) * (maxXTo - minXTo)) + minXTo);

      float minYFrom = GraphRanges.Y;
      float maxYFrom = GraphRanges.MaxExtentY;
      float minYTo = GraphArea.MaxExtentY;
      float maxYTo = GraphArea.Y;

      int y = (int)MathF.Round((((a - minYFrom) / (maxYFrom - minYFrom)) * (maxYTo - minYTo)) + minYTo);

      Point graphPoint = new Point(x, y);
      return graphPoint;
    }

    private MXna.Vector2 MapCellToValues(Point cell)
    {
      float minXFrom = GraphArea.X;
      float maxXFrom = GraphArea.MaxExtentX;
      float minXTo = GraphRanges.X;
      float maxXTo = GraphRanges.MaxExtentX;

      //int x = (int)((((cell.X - minXFrom) / (maxXFrom - minXFrom)) * (maxXTo - minXTo)) + minXTo);
      float x = XAxis[cell.X - GraphArea.MinExtentX];

      float minYFrom = GraphArea.MaxExtentY;
      float maxYFrom = GraphArea.Y;
      float minYTo = GraphRanges.Y;
      float maxYTo = GraphRanges.MaxExtentY;

      //int y = (int)MathF.Round((((cell.Y - minYFrom) / (maxYFrom - minYFrom)) * (maxYTo - minYTo)) + minYTo);

      int y = YAxis[cell.Y - GraphArea.MinExtentY];

      MXna.Vector2 graphPoint = new(x, y);
      return graphPoint;
    }

    private void RenderPointData()
    {
      var TMinus = altitudeRecord.Item1;
      var Altitudes = altitudeRecord.Item2;

      for (int i = 0; i < TMinus.Count; ++i)
      {
        float a = Altitudes[i];
        float t = TMinus[i];
        Point point = MapPointToGraph(a, t);

        Surface.SetGlyph(point.X, point.Y, 249);
      }
    }

    // Mouse Handler
    protected override void OnMouseExit(MouseScreenObjectState state)
    {
      base.OnMouseExit(state);
      mouseHandler.HandleExit(state);
    }
    protected override void OnMouseEnter(MouseScreenObjectState state)
    {
      base.OnMouseEnter(state);
      mouseHandler.HandleEnter(state);
    }

    protected override void OnMouseMove(MouseScreenObjectState state)
    {
      base.OnMouseMove(state);

      if (!GraphArea.Contains(state.SurfaceCellPosition)) return;

      hasUpdate = true;
      mouseHandler.HandleMove(state);
      mouseHandler.RenderMouseCoordinates(state, MapCellToValues(state.SurfaceCellPosition));
    }
  }

  class AltitudeGraphMouseHandler : IRenderStep
  {
    private bool hovering = false;
    private Point mouseCellScreenPosition = new();
    private Point mouseScreenPosition = new();

    private Point axisScreenPosition = new();

    public string Name => "AltitudeGraphMouseHandler";

    public uint SortOrder { get; set; } = 80;


    public AltitudeGraphMouseHandler(ScreenSurface surface, int XAxisPosition, int YAxisPosition)
    {
      surface.Renderer!.Steps.Add(this);
      surface.Renderer!.Steps.Sort(RenderStepComparer.Instance);

      axisScreenPosition = CellToScreen(new(XAxisPosition, YAxisPosition), surface);
    }

    private Point CellToScreen(Point cell, ScreenSurface hoverSurface)
    {
      return (cell - hoverSurface.ViewPosition)
        .SurfaceLocationToPixel(hoverSurface.FontSize.X, hoverSurface.FontSize.Y) + (hoverSurface.FontSize / 2);
    }

    public void HandleExit(MouseScreenObjectState state)
    {
      hovering = (state.IsOnScreenObject && state.ScreenObject is AltitudeGraph) ? false : hovering;
    }
    public void HandleEnter(MouseScreenObjectState state)
    {
      hovering = state.IsOnScreenObject && state.ScreenObject is AltitudeGraph;

      if (!hovering) return;

      mouseCellScreenPosition = CellToScreen(state.SurfaceCellPosition, (ScreenSurface)state.ScreenObject!);
    }

    public void HandleMove(MouseScreenObjectState state)
    {
      if (!hovering) return;

      mouseCellScreenPosition = CellToScreen(state.SurfaceCellPosition, (ScreenSurface)state.ScreenObject!);
      mouseScreenPosition = state.SurfacePixelPosition;
    }

    public void RenderMouseCoordinates(MouseScreenObjectState state, MXna.Vector2 value)
    {
      if (!hovering) return;
      Point printLoc = new(state.SurfaceCellPosition.X + 1, state.SurfaceCellPosition.Y - 1);
      (state.ScreenObject as ScreenSurface)?.Print(printLoc.X, printLoc.Y, "(" + value.X + ", " + value.Y + ")");
    }

    public void RenderOverlay(ScreenSurfaceRenderer renderer, IScreenSurface surface)
    {
      if (!hovering) return;

      SadConsole.Host.Global.SharedSpriteBatch.DrawLine(
        new MXna.Vector2(axisScreenPosition.X, mouseCellScreenPosition.Y), 
        new MXna.Vector2(mouseCellScreenPosition.X, mouseCellScreenPosition.Y),
        MXna.Color.White);
      SadConsole.Host.Global.SharedSpriteBatch.DrawLine(
        new MXna.Vector2(mouseCellScreenPosition.X, axisScreenPosition.Y),
        new MXna.Vector2(mouseCellScreenPosition.X, mouseCellScreenPosition.Y),
        MXna.Color.White);
      SadConsole.Host.Global.SharedSpriteBatch.DrawCircle(new MXna.Vector2(mouseScreenPosition.X, mouseScreenPosition.Y), 5, 8, MXna.Color.Beige);
    }

    // Renderer Interface
    public void SetData(object data) { }
    public void Reset() { }

    public bool Refresh(IRenderer renderer, IScreenSurface screenObject, bool backingTextureChanged, bool isForced)
    {
      return true; // Return true to make sure Composing will be called
    }

    public void Composing(IRenderer renderer, IScreenSurface screenObject) =>
      RenderOverlay((SadConsole.Renderers.ScreenSurfaceRenderer)renderer!, screenObject);

    public void Render(IRenderer renderer, IScreenSurface screenObject) { }

    public void Dispose() => Reset();
    //
  }
}