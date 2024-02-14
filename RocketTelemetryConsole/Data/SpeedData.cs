using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RocketTelemetryConsole.Data
{
  public static class SpeedData
  {
    public static float Speed { get => currentSpeed; private set => currentSpeed = value; }
    private static float currentSpeed = 0;

    public static bool HasUpdate => hasUpdate;
    private static bool hasUpdate = true;

    public static Tuple<List<float>, List<float>> SpeedRecord { get => speedRecord; }
    private static Tuple<List<float>, List<float>> speedRecord = new(new(), new());
    
    public static void CreateDummyData()
    {
      MakeRecord(0, 0);
      MakeRecord(1, 10);
      MakeRecord(2, 20);
      MakeRecord(3.5f, 38);
      MakeRecord(4.5f, 53);
      MakeRecord(5.5f, 53);
      MakeRecord(6, 50);
      MakeRecord(7, 42);
      MakeRecord(8, 30);
      MakeRecord(8.5f, 20);
      MakeRecord(9.5f, 0);
      MakeRecord(10.5f, -20);
      MakeRecord(11.5f, -12);
      MakeRecord(12.5f, -13);
      MakeRecord(13.5f, -12);
      MakeRecord(14f, 0);
    }

    public static void MakeRecord(float time, float speed)
    {
      speedRecord.Item1.Add(time);
      speedRecord.Item2.Add(speed);
    }
  }
}
