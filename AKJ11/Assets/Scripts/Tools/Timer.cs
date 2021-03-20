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
            MonoBehaviour.print("Paused");
        }
    }

    public void Unpause() {
        stopwatch.Start();
        MonoBehaviour.print("Started");
    }

    public string GetString() {
        return stopwatch.Elapsed.ToString(@"hh\:mm\:ss\.ff");
    }

}
