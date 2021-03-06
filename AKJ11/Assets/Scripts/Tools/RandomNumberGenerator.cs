using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RandomNumberGenerator
{

    private static RandomNumberGenerator main;

    private int intSeed;
    private string stringSeed = "";
    public string Seed { get { return stringSeed; } }
    public RandomNumberGenerator() : this(Environment.TickCount) { }
    public RandomNumberGenerator(string seed) : this(seed.GetHashCode()) {
        stringSeed = seed;
    }

    public RandomNumberGenerator(int seed)
    {
        intSeed = seed;
        stringSeed = seed.ToString();
        UnityEngine.Random.InitState(intSeed);
    }

    public static void SetInstance(RandomNumberGenerator rng) {
        main = rng;
    }

    public static RandomNumberGenerator GetInstance() {
        if (main == null) {
            main = new RandomNumberGenerator();
        }
        return main;
    }

    public float Value() {
        return UnityEngine.Random.value;
    }

    public float[] Values(int numberOfValues)
    {
        float[] values = new float[numberOfValues];
        for(int index = 0; index < numberOfValues; index += 1) {
            values[index] = Value();
        }
        return values;
    }

    public float Range(float minInclusive, float maxInclusive)
    {
        //return rng.Next;
        return UnityEngine.Random.Range(minInclusive, maxInclusive);
    }

    public int Range(int minInclusive, int maxExclusive)
    {
        //return rng.Next;
        return UnityEngine.Random.Range(minInclusive, maxExclusive);
    }

}
