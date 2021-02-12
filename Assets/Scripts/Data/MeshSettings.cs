using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class MeshSettings : UpdatableData
{
    public float meshScale = 2.5f;
    public bool useFlatShading;

	public const int nbOfSupportedLOD = 5;
	public const int numSupportedChunkSizes = 9;
	public static readonly int[] supportedChunkSizes = { 48, 72, 96, 120, 144, 168, 192, 216, 240 };
	public const int numSupportedFlatShadedChunkSizes = 3;

	[Range(0, numSupportedChunkSizes - 1)]
	public int chunkSizeIndex;

	[Range(0, numSupportedFlatShadedChunkSizes - 1)]
	public int flatshadedChunkSizeIndex;

    // Num vertices per line rendered at LOD = 0
    // Includes the 2 extra vertices which are excluded from final mesh but used for calculating normals
    public int numVertsPerLine
    {
        get
        {
            return supportedChunkSizes[useFlatShading ? flatshadedChunkSizeIndex : chunkSizeIndex] + 5;
        }
    }

    public float meshWorldSize
    {
        get
        {
            return (numVertsPerLine - 3) * meshScale;
        }
    }
}
