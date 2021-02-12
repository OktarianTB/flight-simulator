using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HeightMapGenerator
{
    public static HeightMap GenerateHeightMap(int width, int height, HeightMapSettings settings, HeightMapSettings simplexSettings, Vector2 sampleCentre)
    {
        float[,] values = Noise.GenerateNoiseMap(width, height, settings.noiseSettings, sampleCentre);
        float[,] simplex = Noise.GenerateSimplexNoiseMap(width, height, simplexSettings.noiseSettings, sampleCentre);
        //float[,] falloffMap = FalloffGenerator.GenerateFalloffMap(width, settings.noiseSettings.seed);

        AnimationCurve heightCurve_threadSafe = new AnimationCurve(settings.heightCurve.keys);
        AnimationCurve simplexHeightCurve_threadSafe = new AnimationCurve(simplexSettings.heightCurve.keys);

        float minValue = int.MaxValue;
        float maxValue = int.MinValue;

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                values[i, j] *= heightCurve_threadSafe.Evaluate(values[i, j] - simplex[i,j] * 0.5f) * settings.heightMultiplier;

                if (values[i, j] > maxValue)
                    maxValue = values[i, j];
                if (values[i, j] < minValue)
                    minValue = values[i, j];
            }
        }

        return new HeightMap(values, minValue, maxValue);
    }


}

public struct HeightMap
{
    public readonly float[,] values;
    public readonly float minValue;
    public readonly float maxValue;

    public HeightMap(float[,] heightMap, float min, float max)
    {
        this.values = heightMap;
        this.minValue = min;
        this.maxValue = max;
    }
}