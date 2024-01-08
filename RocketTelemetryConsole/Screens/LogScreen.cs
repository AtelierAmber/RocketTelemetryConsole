using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadConsole;
using SadConsole.Input;
using SadConsole.UI;
using SadRogue.Primitives;
using static System.Net.Mime.MediaTypeNames;

namespace RocketTelemetryConsole.Screens
{
  public class LogScreen : SadConsole.Console
  {
    public static LogScreen Logger { get; private set; }

    public LogScreen(int width, int height) : base(width, height) 
    {
      Logger = this;
      Cursor.IsVisible = false;
      Log("Log Initialized");
    }

    public override bool ProcessKeyboard(Keyboard keyboard)
    {
      return (Parent != null) ? Parent.ProcessKeyboard(keyboard) : false;
    }

    public void Log(string text)
    {
      Cursor.SetPrintAppearance(Color.White);
      Cursor.Print(text);
      Cursor.NewLine();
    }
    public void Warn(string text)
    {
      Cursor.SetPrintAppearance(Color.Yellow);
      Cursor.Print(text);
      Cursor.NewLine();
    }

    public void Error(string text)
    {
      Cursor.SetPrintAppearance(Color.Red);
      Cursor.Print(text);
      Cursor.NewLine();
    }
  }
}
