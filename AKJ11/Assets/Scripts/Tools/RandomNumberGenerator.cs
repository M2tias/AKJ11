using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RandomNumberGenerator
{

    private static RandomNumberGenerator main;
    private System.Random rng;

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
        rng = new System.Random(seed);
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

    public int Range(int minInclusive, int maxExclusive)
    {
        int result = rng.Next(minInclusive, maxExclusive);
        return result;
    }

}
