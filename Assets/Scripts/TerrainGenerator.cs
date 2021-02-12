using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    const float viewerMoveThreshold = 25f;
    const float sqrviewerMoveThreshold = viewerMoveThreshold * viewerMoveThreshold;

    public LODInfo[] myDetailLevels;
    public int colliderLODIndex;

    public Transform viewer;
    public Material mapMaterial;
    public MeshSettings meshSettings;
    public HeightMapSettings heightMapSettings;
    public HeightMapSettings simplexHeightMapSettings;
    public TextureData textureSettings;

    public Vector2 viewerPosition;
    Vector2 oldViewerPosition;
    float meshWorldSize;
    int chunksVisibleInViewDistance;

    Dictionary<Vector2, TerrainChunk> terrainChunkDictionary = new Dictionary<Vector2, TerrainChunk>();
    List<TerrainChunk> visibleTerrainChunks = new List<TerrainChunk>();

    private void Start()
    {
        textureSettings.ApplyToMaterial(mapMaterial);
        textureSettings.UpdateMeshHeights(mapMaterial, heightMapSettings.minHeight, heightMapSettings.maxHeight);

        float maxViewDistance = myDetailLevels[myDetailLevels.Length - 1].visibleDistanceThreshold;
        meshWorldSize = meshSettings.meshWorldSize;
        chunksVisibleInViewDistance = Mathf.RoundToInt(maxViewDistance / meshWorldSize);

        UdpdateVisibleChunks();
    }

    private void Update()
    {
        viewerPosition = new Vector2(viewer.position.x, viewer.position.z);

        if((oldViewerPosition-viewerPosition).sqrMagnitude > sqrviewerMoveThreshold)
        {
            UdpdateVisibleChunks();
            oldViewerPosition = viewerPosition;
        }

        if(viewerPosition != oldViewerPosition)
        {
            foreach(TerrainChunk chunk in visibleTerrainChunks)
            {
                chunk.UpdateCollisionMesh();
            }
        }

    }

    void UdpdateVisibleChunks()
    {
        HashSet<Vector2> alreadyUpdatedChunkCoords = new HashSet<Vector2>();

        for (int i = visibleTerrainChunks.Count - 1; i >= 0 ; --i)
        {
            visibleTerrainChunks[i].UpdateTerrainChunk();
            alreadyUpdatedChunkCoords.Add(visibleTerrainChunks[i].coordinate);
        }

        int currentChunkCoordX = Mathf.RoundToInt(viewerPosition.x / meshWorldSize);
        int currentChunkCoordY = Mathf.RoundToInt(viewerPosition.y / meshWorldSize);

        for(int yOffset = -chunksVisibleInViewDistance; yOffset <= chunksVisibleInViewDistance; yOffset++)
        {
            for (int xOffset = -chunksVisibleInViewDistance; xOffset <= chunksVisibleInViewDistance; xOffset++)
            {
                Vector2 viewedChunkCoord = new Vector2(currentChunkCoordX + xOffset, currentChunkCoordY + yOffset);

                if (!alreadyUpdatedChunkCoords.Contains(viewedChunkCoord))
                { 
                    if (terrainChunkDictionary.ContainsKey(viewedChunkCoord))
                    {
                        terrainChunkDictionary[viewedChunkCoord].UpdateTerrainChunk();
                    }
                    else
                    {
                        TerrainChunk terrainChunk = new TerrainChunk(viewedChunkCoord, heightMapSettings, simplexHeightMapSettings, meshSettings, myDetailLevels, colliderLODIndex, viewer, transform, mapMaterial);
                        terrainChunkDictionary.Add(viewedChunkCoord, terrainChunk);
                        terrainChunk.OnVisibilityChange += OnTerrainVisibilityChanged;
                        terrainChunk.Load();
                    }
                }
            }
        }
    }

    void OnTerrainVisibilityChanged(TerrainChunk chunk, bool isVisible)
    {
        if(isVisible)
        {
            visibleTerrainChunks.Add(chunk);
        }
        else
        {
            visibleTerrainChunks.Remove(chunk);
        }
    }

}

[System.Serializable]
public struct LODInfo
{
    [Range(0, MeshSettings.nbOfSupportedLOD - 1)]
    public int lod;
    public float visibleDistanceThreshold;
    public float squareVisibleDistThreshold
    {
        get
        {
            return visibleDistanceThreshold * visibleDistanceThreshold;
        }
    }
}
