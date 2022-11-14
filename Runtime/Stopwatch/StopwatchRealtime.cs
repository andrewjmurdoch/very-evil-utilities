using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VED.Utilities
{
    public class StopwatchRealtime : Stopwatch
    {
        public override void Start()
        {
            Running = true;
            Epoch = TimeManager.RealTime;
            End = TimeManager.RealTime;
            TimeManager.Instance.AddStopwatch(this);
            UpdateDebug();
        }

        public override void Stop()
        {
            Running = false;
            End = TimeManager.RealTime;
            TimeManager.Instance.RemoveStopwatch(this);
            UpdateDebug();
        }

        public override void Restart()
        {
            Epoch = TimeManager.RealTime;
            End = TimeManager.RealTime;
            UpdateDebug();
        }

        public override void Tick()
        {
            End = TimeManager.RealTime;
            UpdateDebug();
        }
    }
}