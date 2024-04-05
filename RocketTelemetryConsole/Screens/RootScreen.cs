using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadConsole;
using SadConsole.Input;
using SadRogue.Primitives;
using SadRogue.Primitives.GridViews;

namespace RocketTelemetryConsole.Screens
{
  public class RootScreen : ScreenSurface
  {
    public RootScreen() : base(100, 100) { }
    
    public void Init()
    {
      int width = SadConsole.Host.Global.GraphicsDeviceManager.PreferredBackBufferWidth / Font.GetFontSize(IFont.Sizes.One).X;
      int height = SadConsole.Host.Global.GraphicsDeviceManager.PreferredBackBufferHeight / Font.GetFontSize(IFont.Sizes.One).Y;

      Resize(width, height, true);

      LogScreen logScreen = LogScreen.Logger.Construct(Surface.Width / 2, Surface.Height / 4);
      logScreen.Position = new Point(0, (Surface.Height/4)*3);
      logScreen.IsFocused = true;
      Children.Add(logScreen);

      AltitudeGraph altitudeGraph = new AltitudeGraph(Surface.Width / 2, Surface.Height / 2);
      altitudeGraph.Position = new Point(Surface.Width/2, Surface.Height / 2);
      Children.Add(altitudeGraph);

      GPSLocation gps = new GPSLocation(Surface.Width / 2, 3);
      gps.Position = new Point(Surface.Width / 2, (Surface.Height / 2) - 3);
      Children.Add(gps);

      SpeedGraph speedGraph = new SpeedGraph(Surface.Width / 2, (Surface.Height / 4) - 1);
      speedGraph.Position = new Point(0, Surface.Height / 2);
      Children.Add(speedGraph);
    }

    public override bool ProcessKeyboard(Keyboard keyboard)
    {
      if(keyboard.IsKeyReleased(Keys.Escape) && keyboard.IsKeyDown(Keys.LeftShift)){
        SadConsole.Game.Instance.MonoGameInstance.Exit();
      }
      return base.ProcessKeyboard(keyboard);
    }

  }
}
