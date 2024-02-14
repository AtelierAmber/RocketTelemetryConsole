using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RocketTelemetryConsole
{
  public static class Utils
  {
    public static class Math
    {
      public static int DivisorCount(float num, float divisor, float min)
      {
        return (int)(MathF.Ceiling((MathF.Log(num) - MathF.Log(min)) / MathF.Log(divisor)));
      }
    }
  }
}
