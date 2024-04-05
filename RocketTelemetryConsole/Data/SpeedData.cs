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
      PublishRecord(0, 0);
      PublishRecord(1, 10);
      PublishRecord(2, 20);
      PublishRecord(3.5f, 38);
      PublishRecord(4.5f, 53);
      PublishRecord(5.5f, 53);
      PublishRecord(6, 50);
      PublishRecord(7, 42);
      PublishRecord(8, 30);
      PublishRecord(8.5f, 20);
      PublishRecord(9.5f, 0);
      PublishRecord(10.5f, -20);
      PublishRecord(11.5f, -12);
      PublishRecord(12.5f, -13);
      PublishRecord(13.5f, -12);
      PublishRecord(14f, 0);
    }

    public static void PublishRecord(float time, float speed)
    {
      speedRecord.Item1.Add(time);
      speedRecord.Item2.Add(speed);
    }
  }
}
