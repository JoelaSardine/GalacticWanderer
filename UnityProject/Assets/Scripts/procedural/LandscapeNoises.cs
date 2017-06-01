using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public enum FilterFunction
{
    EXP,
    LOG,
    GAMMA,
    DEFAULT
}

[Serializable]
public struct NoiseData
{
    public Vector2 frequency;
    public float weight;
    public FastNoise currentNoise;
    public int seed;
}

public class LandscapeNoises : MonoBehaviour
{
    private static LandscapeNoises instance;
    public NoiseData[] noiseDataList;
    public FilterFunction filter;
    public int a;
    public float b;

    public void Start()
    {
        for(int i = 0; i< noiseDataList.Length; ++i)
        {
            noiseDataList[i].currentNoise = new FastNoise(noiseDataList[i].seed);
            noiseDataList[i].currentNoise.SetNoiseType(FastNoise.NoiseType.Perlin);
        }
        instance = this;
    }

    float CombineNoise(float x, float y)
    {
        float result = 0;
        foreach(NoiseData noiseData in noiseDataList)
        {
            result += noiseData.weight * noiseData.currentNoise.GetNoise(noiseData.frequency.x * x, noiseData.frequency.y * y);
        }
        switch (filter)
        {
            case FilterFunction.EXP:
                return Mathf.Pow(result, a); // interessant
            case FilterFunction.LOG:
                return Mathf.Log(1 + result);
            case FilterFunction.GAMMA:
                return Mathf.Gamma(result, a, b);
            case FilterFunction.DEFAULT:
                return result;
            default:
                return result;
    }
        
    }

    public static float GetNoise(float x, float y)
    {
        return instance.CombineNoise(x, y);
    }
}



