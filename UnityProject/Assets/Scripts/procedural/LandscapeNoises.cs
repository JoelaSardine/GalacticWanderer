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
    public NoiseData[] elevationNoiseList;
    public NoiseData[] moistureNoiseList;
    public FilterFunction filter;
    public int a;
    public float b;

    public void Start()
    {
        for(int i = 0; i< elevationNoiseList.Length; ++i)
        {
            elevationNoiseList[i].currentNoise = new FastNoise(elevationNoiseList[i].seed);
            elevationNoiseList[i].currentNoise.SetNoiseType(FastNoise.NoiseType.Perlin);
        }
        for (int i = 0; i < moistureNoiseList.Length; ++i)
        {
            moistureNoiseList[i].currentNoise = new FastNoise(moistureNoiseList[i].seed);
            moistureNoiseList[i].currentNoise.SetNoiseType(FastNoise.NoiseType.Perlin);
        }
        instance = this;
    }

    public static float GetMaxNoise(NoiseData[] noiseDataList, FilterFunction filter, int a, float b)
    {
        float result = 0;
        foreach(NoiseData noiseData in noiseDataList)
        {
            result += noiseData.weight;
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

    static float GetMinNoise(NoiseData[] noiseDataList)
    {
        return 0;
    }

    float CombineNoise(float x, float y, NoiseData[] noiseDataList, FilterFunction f)
    {
        float result = 0;
        foreach (NoiseData noiseData in noiseDataList)
        {
            result += noiseData.weight * noiseData.currentNoise.GetNoise(noiseData.frequency.x * x, noiseData.frequency.y * y);
        }
        switch (f)
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

    float CombineElevationNoise(float x, float y)
    {
        return CombineNoise(x, y, elevationNoiseList, filter);      
    }

    public static float GetElevationNoise(float x, float y)
    {
        return instance.CombineElevationNoise(x, y);
    }

    float CombineMoistureNoise(float x, float y)
    {
        return CombineNoise(x, y, moistureNoiseList, FilterFunction.DEFAULT);
    }

    public static float GetMoistureNoise(float x, float y)
    {
        return instance.CombineMoistureNoise(x, y);
    }

    public static float GetMaxMoisture()
    {
        return GetMaxNoise(instance.moistureNoiseList, instance.filter, instance.a, instance.b);
    }
}



