using UnityEngine;
using System;
using Cysharp.Threading.Tasks;

[CreateAssetMenu(fileName = "DebugConfig", menuName = "Configs/DebugConfig", order = 0)]
public class DebugConfig : ScriptableObject
{

    [field: SerializeField]
    [field: Range(1, 1000)]
    public int GenerationDelayMs {get; private set;} = 10;
    private TimeSpan generationDelay = TimeSpan.Zero;
    [field: SerializeField]
    public bool DelayGeneration {get; private set;} = true;

    public TimeSpan GenerationDelay
    {
        get
        {
            if (generationDelay.Milliseconds != GenerationDelayMs)
            {
                generationDelay = TimeSpan.FromMilliseconds(GenerationDelayMs);
            }
            return generationDelay;
        }
    }

    public async UniTask DelayIfCounterFinished(DelayCounter counter)
    {
        if (DelayGeneration)
        {
            counter.Increment();
            if (counter.IsFinished())
            {
                if (DelayGeneration)
                {
                    await UniTask.Delay(GenerationDelay);
                }
                else
                {
                    await UniTask.NextFrame();
                }
            }
        }
    }
}

public class DelayCounter
{
    public int stepsDone = 0;
    public int maxSteps;

    public DelayCounter(int steps = 0)
    {
        maxSteps = steps;
    }

    public void Increment()
    {
        stepsDone += 1;
    }
    public bool IsFinished()
    {
        if (stepsDone >= maxSteps)
        {
            stepsDone = 0;
            return true;
        }
        return false;
    }
}