using UnityEngine;
using System.Collections.Generic;

public class InfiniteMapGenerator : MonoBehaviour
{
    [Header("Referências")]
    [Tooltip("Arraste aqui o Transform do Player ou da MainCamera")]
    [SerializeField] private Transform player;
    [Tooltip("Arraste aqui o prefab do chunk para instanciar novos")]
    [SerializeField] private GameObject chunkPrefab;

    [Header("Origem do mapa em world coords")]
    [Tooltip("Se o chunk (0,0) já estiver em outro lugar, defina aqui")]
    [SerializeField] private Vector2 mapOrigin = new Vector2(9.029942f, 0.9167452f);

    [Header("Tamanho do Chunk (unidades)")]
    [SerializeField] private int chunkWidth = 40;
    [SerializeField] private int chunkHeight = 40;

    [Header("Raio de renderização (chunks)")]
    [SerializeField, Range(1, 8)] private int renderDistance = 4;

    private Vector2Int currentChunk;
    private readonly Dictionary<Vector2Int, GameObject> activeChunks = new Dictionary<Vector2Int, GameObject>();

    private void Awake()
    {
        // Remove qualquer filho existente (incluindo chunk estático)
        foreach (Transform child in transform)
            Destroy(child.gameObject);
        activeChunks.Clear();
    }

    private void Start()
    {
        if (player == null || chunkPrefab == null)
        {
            Debug.LogError("InfiniteMapGenerator: Player ou chunkPrefab não estão atribuídos!");
            enabled = false;
            return;
        }

        // Define o chunk inicial de acordo com a posição do player
        currentChunk = WorldToChunk(player.position);

        // Gera todos os chunks iniciais ao redor
        for (int x = -renderDistance; x <= renderDistance; x++)
            for (int y = -renderDistance; y <= renderDistance; y++)
                TrySpawn(new Vector2Int(currentChunk.x + x, currentChunk.y + y));
    }

    private void Update()
    {
        Vector2Int newChunk = WorldToChunk(player.position);
        if (newChunk != currentChunk)
        {
            OnChunkChanged(currentChunk, newChunk);
            currentChunk = newChunk;
        }
    }

    // Converte posição world em coordenada de chunk, levando em conta a origem (offset)
    private Vector2Int WorldToChunk(Vector3 pos)
    {
        float localX = pos.x - mapOrigin.x;
        float localY = pos.y - mapOrigin.y;
        int cx = Mathf.FloorToInt(localX / chunkWidth);
        int cy = Mathf.FloorToInt(localY / chunkHeight);
        return new Vector2Int(cx, cy);
    }

    private void OnChunkChanged(Vector2Int oldC, Vector2Int newC)
    {
        int dx = newC.x - oldC.x;
        int dy = newC.y - oldC.y;

        // Spawn da nova borda em X
        if (dx != 0)
        {
            int spawnX = newC.x + dx * renderDistance;
            for (int y = newC.y - renderDistance; y <= newC.y + renderDistance; y++)
                TrySpawn(new Vector2Int(spawnX, y));
        }

        // Spawn da nova borda em Y
        if (dy != 0)
        {
            int spawnY = newC.y + dy * renderDistance;
            for (int x = newC.x - renderDistance; x <= newC.x + renderDistance; x++)
                TrySpawn(new Vector2Int(x, spawnY));
        }

        // Spawn do canto diagonal (quando dx e dy são não-zero)
        if (dx != 0 && dy != 0)
        {
            TrySpawn(new Vector2Int(
                newC.x + dx * renderDistance,
                newC.y + dy * renderDistance
            ));
        }

        // Destrói chunks fora do raio de renderização
        List<Vector2Int> toRemove = new List<Vector2Int>();
        foreach (var kv in activeChunks)
        {
            var coord = kv.Key;
            if (Mathf.Abs(coord.x - newC.x) > renderDistance || Mathf.Abs(coord.y - newC.y) > renderDistance)
            {
                Destroy(kv.Value);
                toRemove.Add(coord);
            }
        }
        foreach (var coord in toRemove)
            activeChunks.Remove(coord);
    }

    // Tenta spawnar apenas se ainda não existir aquele chunk
    private void TrySpawn(Vector2Int coord)
    {
        if (activeChunks.ContainsKey(coord)) return;
        SpawnChunk(coord);
    }

    // Instancia o chunk no world, aplicando o offset de origem
    private void SpawnChunk(Vector2Int coord)
    {
        float worldX = coord.x * chunkWidth + mapOrigin.x;
        float worldY = coord.y * chunkHeight + mapOrigin.y;
        Vector3 worldPos = new Vector3(worldX, worldY, 0f);
        GameObject chunk = Instantiate(chunkPrefab, worldPos, Quaternion.identity, transform);
        activeChunks[coord] = chunk;
    }
}