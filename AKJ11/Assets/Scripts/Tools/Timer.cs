using System.Diagnostics;
using UnityEngine;
public class Timer
{

    Stopwatch stopwatch;
    public Timer() {
        stopwatch = Stopwatch.StartNew();
    }
    public void Pause() {
        if (stopwatch.IsRunning) {
            stopwatch.Stop();
        }
    }

    public void Unpause() {
        stopwatch.Start();
    }

    public string GetString() {
        return stopwatch.Elapsed.ToString(@"mm\:ss\.ff");
    }

    public double GetTime() {
        return stopwatch.Elapsed.TotalMilliseconds;
    }

}
