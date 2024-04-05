using MonoGame.Extended;
using RocketTelemetryConsole.Data;
using SadConsole;
using SadConsole.Input;
using SadConsole.Renderers;
using SadRogue.Primitives;

using MXna = Microsoft.Xna.Framework;

namespace RocketTelemetryConsole.Screens
{
  public abstract class GraphScreen : ScreenSurface
  {
    public GraphScreen(int width, int height, int maxXTicks, int maxYTicks, float xGrain, float yGrain) : base(width, height)
    {
      GraphArea = new((7, 2), (Surface.Width - 3, Surface.Height - 4));
      mouseHandler = new GraphMouseHandler(this, GraphArea.X, GraphArea.Y);
      graphLineRenderer = new GraphLineRenderer(this);
      MaxXTicks = maxXTicks;
      MaxYTicks = maxYTicks;
      XGranularity = xGrain;
      YGranularity = yGrain;
    }

    public Rectangle GraphArea { get; private set; }
    public float[] XAxis;
    public int[] YAxis;
    public Rectangle GraphRanges { get; private set; }

    protected bool hasUpdate = true;

    protected int MaxXTicks = 10;
    protected int MaxYTicks = 8;
    protected float XGranularity = 1.0f;
    protected float YGranularity = 500.0f;

    protected GraphMouseHandler mouseHandler;
    protected GraphLineRenderer graphLineRenderer;


    public override void Render(TimeSpan delta)
    {
      base.Render(delta);

      if (hasUpdate)
      {
        Surface.Clear();
        RenderBorder();
        RenderTitle();
        RenderAxis();
        RenderData();
        hasUpdate = false;
      }
    }

    protected virtual void RenderBorder()
    {
      Surface.DrawBox(Surface.Area, ShapeParameters.CreateStyledBoxThin(new Color(60, 60, 60)));
    }

    protected abstract void RenderTitle();

    protected virtual void RenderAxis()
    {
      Surface.DrawLine((GraphArea.X - 1, GraphArea.MaxExtentY + 1), (GraphArea.MaxExtentX, GraphArea.MaxExtentY + 1), 196); // X Axis
      Surface.DrawLine((GraphArea.X - 1, GraphArea.MaxExtentY + 1), (GraphArea.X - 1, GraphArea.Y + 1), 179); // Y Axis
      Surface.SetGlyph(GraphArea.X - 1, GraphArea.MaxExtentY + 1, 192);
    }

    protected abstract void RenderData();

    protected void DevelopAndPrintAxisTicks(List<float> fullXAxis, List<float> fullYAxis)
    {
      float xScalar, minX, maxX;
      int xDivisorCount, numXTicks;

      if (fullXAxis.Count <= 0)
      {
        xScalar = 2.0f;
        xDivisorCount = 0;
        numXTicks = 1;
        minX = 0;
        maxX = minX + (numXTicks * XGranularity);
      }
      else
      {
        float fullXTicksNum = ((fullXAxis.Last() - fullXAxis.First()) / XGranularity);
        xScalar = 2.0f;
        xDivisorCount = Utils.Math.DivisorCount(fullXTicksNum, xScalar, MaxXTicks);
        numXTicks = (int)MathF.Ceiling(fullXTicksNum / MathF.Max(1.0f, (xDivisorCount * xScalar)));
        minX = fullXAxis.First();
        maxX = minX + (numXTicks * MathF.Max(1.0f, xDivisorCount * xScalar) * XGranularity);
      }
      XAxis = new float[GraphArea.Width];

      float yScalar, minY, maxY;
      int yDivisorCount, numYTicks;

      if (fullYAxis.Count <= 0)
      {
        yScalar = 2.0f;
        yDivisorCount = 0;
        numYTicks = 1;
        minY = 0;
        maxY = minY + (numYTicks * YGranularity);
      }
      else
      {
        minY = (MathF.Floor(fullYAxis.Min() / YGranularity)) * YGranularity;
        float fullYTicksNum = ((fullYAxis.Max() - minY) / YGranularity);
        yScalar = 2.0f;
        yDivisorCount = Utils.Math.DivisorCount(fullYTicksNum, yScalar, MaxYTicks);
        numYTicks = (int)MathF.Ceiling(fullYTicksNum / MathF.Max(1.0f, (yDivisorCount * yScalar)));
        maxY = minY + (numYTicks * MathF.Max(1.0f, yDivisorCount * yScalar) * YGranularity);
      }
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
            for (int y = prevYLoc; y >= yTickLoc; --y)
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

    private Point MapPointToGraph(float x, float y)
    {
      float minXFrom = GraphRanges.X;
      float maxXFrom = GraphRanges.MaxExtentX;
      float minXTo = GraphArea.X;
      float maxXTo = GraphArea.MaxExtentX;

      int xCell = (int)MathF.Round((((x - minXFrom) / (maxXFrom - minXFrom)) * (maxXTo - minXTo)) + minXTo);

      float minYFrom = GraphRanges.Y;
      float maxYFrom = GraphRanges.MaxExtentY;
      float minYTo = GraphArea.MaxExtentY;
      float maxYTo = GraphArea.Y;

      int yCell = (int)MathF.Round((((y - minYFrom) / (maxYFrom - minYFrom)) * (maxYTo - minYTo)) + minYTo);

      Point graphPoint = new Point(xCell, yCell);
      return graphPoint;
    }

    private MXna.Vector2 MapCellToValues(Point cell)
    {
      float minXFrom = GraphArea.X;
      float maxXFrom = GraphArea.MaxExtentX;
      float minXTo = GraphRanges.X;
      float maxXTo = GraphRanges.MaxExtentX;

      float x = XAxis[cell.X - GraphArea.MinExtentX];

      float minYFrom = GraphArea.MaxExtentY;
      float maxYFrom = GraphArea.Y;
      float minYTo = GraphRanges.Y;
      float maxYTo = GraphRanges.MaxExtentY;

      int y = YAxis[cell.Y - GraphArea.MinExtentY];

      MXna.Vector2 graphPoint = new(x, y);
      return graphPoint;
    }

    protected void RenderPointData(List<float> xValues, List<float> yValues)
    {
      graphLineRenderer.PreparePointData();

      for (int i = 0; i < xValues.Count; ++i)
      {
        float x = xValues[i];
        float y = yValues[i];
        Point point = MapPointToGraph(x, y);

        graphLineRenderer.AddPointData(point, this);

        Surface.SetGlyph(point.X, point.Y, 249);
      }
      graphLineRenderer.FinallizePointData();
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

  public class GraphLineRenderer : IRenderStep
  {
    private bool prepared = false;

    private List<MXna.Vector2> pointData = new();

    public string Name => "GraphLineRenderer";

    public uint SortOrder { get; set; } = 80;

    public GraphLineRenderer(ScreenSurface surface)
    {
      surface.Renderer!.Steps.Add(this);
      surface.Renderer!.Steps.Sort(RenderStepComparer.Instance);
    }
    private MXna.Vector2 CellToScreen(Point cell, ScreenSurface hoverSurface)
    {
      Point pixelLoc = (cell - hoverSurface.ViewPosition)
        .SurfaceLocationToPixel(hoverSurface.FontSize.X, hoverSurface.FontSize.Y) + (hoverSurface.FontSize / 2);
      return new(pixelLoc.X, pixelLoc.Y);
    }


    // Prepares the point data for updates
    public void PreparePointData()
    {
      pointData.Clear();
      prepared = true;
    }

    public void AddPointData(Point cell, ScreenSurface surface)
    {
      if (!prepared)
      {
        LogScreen.Logger.Warn("Trying to add point data without preparing point data");
        return;
      }
      pointData.Add(CellToScreen(cell, surface));
    }

    public void FinallizePointData()
    {
      prepared = false;
    }

    public void RenderOverlay(ScreenSurfaceRenderer renderer, IScreenSurface surface)
    {
      if (prepared) return;

      for (int i = 1; i < pointData.Count; ++i)
      {
        SadConsole.Host.Global.SharedSpriteBatch.DrawLine(
        pointData[i - 1],
        pointData[i],
        MXna.Color.WhiteSmoke);
      }
    }

    public bool Refresh(IRenderer renderer, IScreenSurface screenObject, bool backingTextureChanged, bool isForced)
    {
      return true; // Return true to make sure Composing will be called
    }

    public void Composing(IRenderer renderer, IScreenSurface screenObject) =>
      RenderOverlay((SadConsole.Renderers.ScreenSurfaceRenderer)renderer!, screenObject);

    public void Dispose()
    {
    }

    public void Render(IRenderer renderer, IScreenSurface screenObject)
    {
    }

    public void Reset()
    {
    }

    public void SetData(object data)
    {
      throw new NotImplementedException();
    }
  }

  public class GraphMouseHandler : IRenderStep
  {
    private bool hovering = false;
    private Point mouseCellScreenPosition = new();
    private Point mouseScreenPosition = new();

    private Point axisScreenPosition = new();

    public string Name => "GraphMouseHandler";

    public uint SortOrder { get; set; } = 80;


    public GraphMouseHandler(ScreenSurface surface, int XAxisPosition, int YAxisPosition)
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
      hovering = (state.IsOnScreenObject && state.ScreenObject is GraphScreen) ? false : hovering;
    }
    public void HandleEnter(MouseScreenObjectState state)
    {
      hovering = state.IsOnScreenObject && state.ScreenObject is GraphScreen;

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
