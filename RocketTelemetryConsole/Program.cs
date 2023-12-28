using SadConsole;
using SadConsole.Configuration;

namespace RocketTelemetryConsole
{
  public class Program
  {
    public static void Main()
    {
      SadConsole.Settings.WindowTitle = "Telemetry Console";

      SadConsole.Settings.AllowWindowResize = false;
      SadConsole.Settings.ResizeMode = SadConsole.Settings.WindowResizeOptions.None;

      Builder startup = new Builder()
        .IsStartingScreenFocused(false)
        .ConfigureFonts("./Fonts/Cheepicus12.font")
        ;

      SadConsole.Game.Create(startup);
      SadConsole.Game.Instance.Started += Init;
      SadConsole.Game.Instance.Run();
      SadConsole.Game.Instance.Dispose();
    }

    public static void Init(object? sender, SadConsole.GameHost host)
    {
      //SadConsole.Game.Instance.ToggleFullScreen();
    }
  }
}
