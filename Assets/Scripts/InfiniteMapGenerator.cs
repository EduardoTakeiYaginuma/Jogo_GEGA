// InfiniteMapGenerator.cs
using UnityEngine;
using System.Collections.Generic;

public class InfiniteMapGenerator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Transform  player;
    [SerializeField] GameObject chunkPrefab;

    [Header("Map origin (world)")]
    [SerializeField] Vector2 mapOrigin = new(9.029942f, 0.9167452f);

    [Header("Chunk size (world units)")]
    [SerializeField] int chunkWidth  = 40;
    [SerializeField] int chunkHeight = 40;

    [Header("Extra padding (chunks)")]
    [SerializeField] int padding = 1;   // 1-chunk de folga além da tela

    readonly Dictionary<Vector2Int, GameObject> active = new();
    readonly Queue<GameObject> pool = new();

    Vector2Int currentChunk;
    int        renderDist;

    /* ---------------- LIFECYCLE ---------------- */

    void Awake()
    {
        foreach (Transform t in transform) Destroy(t.gameObject);
        active.Clear();
    }

    void Start()
    {
        if (!player || !chunkPrefab) { enabled = false; return; }

        renderDist   = CalcRenderDist();
        currentChunk = WorldToChunk(player.position);
        SpawnInitialRing();
    }

    void Update()
    {
        int desired = CalcRenderDist();
        if (desired != renderDist) renderDist = desired;

        Vector2Int nc = WorldToChunk(player.position);
        if (nc == currentChunk) return;

        OnChunkChanged(currentChunk, nc);
        currentChunk = nc;
    }

    /* ---------------- CORE ---------------- */

    Vector2Int WorldToChunk(Vector3 pos)
    {
        float lx = pos.x - mapOrigin.x;
        float ly = pos.y - mapOrigin.y;
        return new Vector2Int(
            Mathf.FloorToInt(lx / chunkWidth),
            Mathf.FloorToInt(ly / chunkHeight)
        );
    }

    int CalcRenderDist()
    {
        Camera cam = Camera.main;
        float halfH = cam.orthographicSize;
        float halfW = halfH * cam.aspect;
        float maxExtent = Mathf.Max(halfW, halfH);
        return Mathf.CeilToInt(maxExtent / Mathf.Min(chunkWidth, chunkHeight)) + padding;
    }

    void SpawnInitialRing()
    {
        for (int dx = -renderDist; dx <= renderDist; dx++)
            for (int dy = -renderDist; dy <= renderDist; dy++)
                TrySpawn(new Vector2Int(currentChunk.x + dx,
                                        currentChunk.y + dy));
    }

    void OnChunkChanged(Vector2Int oldC, Vector2Int newC)
    {
        int dx = newC.x - oldC.x;
        int dy = newC.y - oldC.y;

        if (dx != 0)
        {
            int sx = newC.x + dx * renderDist;
            for (int y = newC.y - renderDist; y <= newC.y + renderDist; y++)
                TrySpawn(new Vector2Int(sx, y));
        }
        if (dy != 0)
        {
            int sy = newC.y + dy * renderDist;
            for (int x = newC.x - renderDist; x <= newC.x + renderDist; x++)
                TrySpawn(new Vector2Int(x, sy));
        }
        if (dx != 0 && dy != 0)
            TrySpawn(new Vector2Int(newC.x + dx * renderDist,
                                    newC.y + dy * renderDist));

        List<Vector2Int> toRemove = new();
        foreach (var kv in active)
        {
            if (Mathf.Abs(kv.Key.x - newC.x) > renderDist ||
                Mathf.Abs(kv.Key.y - newC.y) > renderDist)
            {
                Recycle(kv.Value);
                toRemove.Add(kv.Key);
            }
        }
        foreach (var c in toRemove) active.Remove(c);
    }

    /* ---------------- SPAWN / POOL ---------------- */

    void TrySpawn(Vector2Int coord)
    {
        if (active.ContainsKey(coord)) return;

        float wx = coord.x * chunkWidth  + mapOrigin.x;
        float wy = coord.y * chunkHeight + mapOrigin.y;
        Vector3 wpos = new(wx, wy, 0f);

        GameObject chunk = pool.Count > 0 ? pool.Dequeue()
                                          : Instantiate(chunkPrefab);
        chunk.transform.SetParent(transform);
        chunk.transform.position = wpos;
        chunk.SetActive(true);

        Bounds bounds = new(
            new Vector3(wx + chunkWidth * 0.5f,
                        wy + chunkHeight * 0.5f,
                        0f),
            new Vector3(chunkWidth, chunkHeight, 1f));

        var tc = chunk.GetComponentInChildren<TerrainChunk>();
        if (tc) tc.Configure(bounds);

        XPSpawnManager.Instance?.SpawnOrbsInChunk(bounds, chunk.transform);

        active[coord] = chunk;
    }

    void Recycle(GameObject go)
    {
        go.SetActive(false);
        pool.Enqueue(go);
    }
}