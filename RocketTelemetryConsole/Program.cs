using RocketTelemetryConsole.Data.Serial;
using RocketTelemetryConsole.Screens;
using SadConsole;
using SadConsole.Configuration;

namespace RocketTelemetryConsole
{
  public class Program
  {
    private const bool ENABLE_TESTING = true;
    private const int BAUD_RATE = 3500;
    private static SerialCommunicator? communicator = null;

    public static void Main()
    {
      SadConsole.Settings.WindowTitle = "Telemetry Console";

      SadConsole.Settings.AllowWindowResize = false;
      SadConsole.Settings.ResizeMode = SadConsole.Settings.WindowResizeOptions.None;

      Builder startup = new Builder()
        .IsStartingScreenFocused(false)
        .SetStartingScreen<RootScreen>()
        .ConfigureFonts("./Fonts/Cheepicus12.font")
        ;

      SadConsole.Game.Create(startup);
      SadConsole.Game.Instance.Started += Init;
      SadConsole.Game.Instance.FrameUpdate += UpdateCommunicator;
      SadConsole.Game.Instance.Ending += Dispose;
      SadConsole.Game.Instance.Run();
      SadConsole.Game.Instance.Dispose();
    }

    public static void Init(object? sender, SadConsole.GameHost host)
    {
      //SadConsole.Game.Instance.ToggleFullScreen();
      SadConsole.Game.Instance.ResizeWindow(1900, 1000);
      (SadConsole.Game.Instance.Screen as RootScreen)?.Init();

      communicator = (ENABLE_TESTING) ? new TestSerialCommunicator() : new SerialCommunicator();
      communicator!.Start(BAUD_RATE);
    }

    public static void UpdateCommunicator(object? sender, SadConsole.GameHost host)
    {
      communicator?.Update();
    }

    public static void Dispose(object? sender, SadConsole.GameHost host)
    {
      communicator?.End();
    }
  }
}
