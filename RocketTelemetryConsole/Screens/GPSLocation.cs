using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadConsole;

namespace RocketTelemetryConsole.Screens
{
  public class GPSLocation : ScreenSurface
  {
    public GPSLocation(int width, int height) : base(width, height)
    {
      string locStr = "Lat: 21.2 Long: 23.3";
      Surface.Print((Surface.Width / 2) - (locStr.Length / 2), 1, locStr);
    }
  }
}
