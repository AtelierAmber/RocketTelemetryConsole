using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using RocketTelemetryConsole.Screens;

namespace RocketTelemetryConsole.Data.Serial
{
  public class TestSerialCommunicator : SerialCommunicator
  {
    private List<FlightInfo> testData = new();
    DateTime startTime;
    int nextTime = 0;

    public TestSerialCommunicator() 
    {
      for(int i = 0; i < 14; ++i)
      {
        AddTestData(i, altitude: (float)(5280 + (2500*(1+(-Math.Cos(0.5*i))))));
      }
    }

    public override void Start(int baudRate)
    {
      LogScreen.Logger.Log("Initializing Testing Forwarder");
      running = true;
      startTime = DateTime.Now;
    }

    private void AddTestData(float tminus, float? altitude = null, float? speed = null)
    {
      testData.Add(new FlightInfo { 
        TMinus = tminus,
        Altitude = altitude,
        Speed = speed
      });
    }

    public override void Update()
    {
      if (running)
      {
        TimeSpan TMinus = DateTime.Now - startTime;
        if ((int)TMinus.TotalSeconds >= nextTime && nextTime < testData.Count)
        {
          for (int i = 0; i < ((int)TMinus.TotalSeconds - nextTime) + 1 && nextTime < testData.Count; ++i)
          {
            FlightInfo info = testData[nextTime++];

            if (info.Altitude.HasValue)
            {
              AltitudeData.PublishRecord(info.TMinus, info.Altitude.Value);
            }
            if (info.Speed.HasValue)
            {
              SpeedData.PublishRecord(info.TMinus, info.Speed.Value);
            }
          }
        }
      }
    }
  }
}
