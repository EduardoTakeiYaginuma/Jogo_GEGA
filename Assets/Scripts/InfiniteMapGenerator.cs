using UnityEngine;
using System.Collections.Generic;

public class InfiniteMapGenerator : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField] Transform player;
    [SerializeField] GameObject chunkPrefab;

    [Header("Configurações")]
    [SerializeField] int chunkWidth  = 40;  // largura em unidades
    [SerializeField] int chunkHeight = 35;  // altura em unidades
    [SerializeField, Range(1, 5)] int renderDistance = 4;

    readonly Dictionary<Vector2Int, GameObject> activeChunks = new();

    void Update()
    {
        var playerChunk = new Vector2Int(
            Mathf.FloorToInt(player.position.x / chunkWidth),
            Mathf.FloorToInt(player.position.y / chunkHeight)
        );

        for (int x = -renderDistance; x <= renderDistance; x++)
            for (int y = -renderDistance; y <= renderDistance; y++)
            {
                var coord = playerChunk + new Vector2Int(x, y);
                if (!activeChunks.ContainsKey(coord))
                    SpawnChunk(coord);
            }

        var toRemove = new List<Vector2Int>();
        foreach (var kv in activeChunks)
        {
            if (Mathf.Abs(kv.Key.x - playerChunk.x) > renderDistance ||
                Mathf.Abs(kv.Key.y - playerChunk.y) > renderDistance)
            {
                Destroy(kv.Value);
                toRemove.Add(kv.Key);
            }
        }
        toRemove.ForEach(k => activeChunks.Remove(k));
    }

    void SpawnChunk(Vector2Int coord)
    {
        Vector3 worldPos = new Vector3(
            coord.x * chunkWidth,
            coord.y * chunkHeight,
            0f
        );
        var chunk = Instantiate(chunkPrefab, worldPos, Quaternion.identity, transform);
        activeChunks.Add(coord, chunk);
    }
}