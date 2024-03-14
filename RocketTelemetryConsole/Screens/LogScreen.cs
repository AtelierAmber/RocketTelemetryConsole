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
    public static LogScreen Logger {
      get
      {
        if (instance == null)
        {
          instance = new LogScreen();
        }
        return instance;
      }
    }
    private static LogScreen? instance = null;

    private static bool constructed = false;
    private List<KeyValuePair<Color, string>> bufferLog = new();

    private LogScreen() : base(0, 0) { constructed = false; }

    public LogScreen Construct(int width, int height) 
    {
      Resize(width, height, true);
      Cursor.IsVisible = false;
      constructed = true;
      Log("Log Initialized");
      ClearBuffer();
      return this;
    }

    public override bool ProcessKeyboard(Keyboard keyboard)
    {
      return (Parent != null) ? Parent.ProcessKeyboard(keyboard) : false;
    }

    private void ClearBuffer()
    {
      foreach(var val in bufferLog)
      {
        Cursor.SetPrintAppearance(val.Key);
        Cursor.Print(val.Value);
        Cursor.NewLine();
      }
    }

    public void Log(string text)
    {
      if (!constructed)
      {
        bufferLog.Add(new(Color.White, text));
        return;
      }

      Cursor.SetPrintAppearance(Color.White);
      Cursor.Print(text);
      Cursor.NewLine();
    }
    public void Warn(string text)
    {
      if (!constructed)
      {
        bufferLog.Add(new(Color.Yellow, text));
        return;
      }

      Cursor.SetPrintAppearance(Color.Yellow);
      Cursor.Print(text);
      Cursor.NewLine();
    }

    public void Error(string text)
    {
      if (!constructed)
      {
        bufferLog.Add(new(Color.Red, text));
        return;
      }

      Cursor.SetPrintAppearance(Color.Red);
      Cursor.Print(text);
      Cursor.NewLine();
    }
  }
}
