using SardineTools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandscapeConstants {

    public const int VERTEX_PER_SIDE = 201;
    public const float LANDSCAPE_SIZE = 100.0f;

    // TODO : we need LOD_MAX

    public static readonly Interval HEIGHT_INTERVAL = new Interval(-100, 100);
    public static readonly Interval[] BIOMES_HEIGHT =
    {
        new Interval(-100, -10),
        new Interval(0, 20),
        new Interval(21, 30),
        new Interval(31, 40),
        new Interval(41, 50),
        new Interval(51, 200)
    };

    public const int ATLAS_LINES = 2;
    public const int ATLAS_COLUMNS = 3;
    public const int TEXTURE_RESOLUTION = 1024;

    public static FastNoise NOISE = new FastNoise(1337);

    static LandscapeConstants()
    {
        NOISE.SetNoiseType(FastNoise.NoiseType.Perlin);
    }

    public const int THREAD_POOL_SIZE = 2;
    public const float MAX_FLIGHT_HEIGHT = 100.0f;
    public const float DISCRETE_Y_UNIT = 10;

    /// <summary>
    /// Minimum map diameter
    /// MUST be an ODD number
    /// </summary>
    public const int MIN_MAP_SIZE = 3;

    /// <summary>
    /// Maximum map diameter
    /// MUST be an ODD number
    /// </summary>
    public const int MAX_MAP_SIZE = 4;
}
