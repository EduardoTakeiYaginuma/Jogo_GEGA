using UnityEngine;

public class TerrainChunk : MonoBehaviour
{
    public static System.Action<TerrainChunk> OnChunkRendered;

    Bounds myBounds;
    bool   hasFired;

    public void Configure(Bounds b) => myBounds = b;

    public Bounds Bounds => myBounds;

    void OnBecameVisible()
    {
        if (hasFired) return;
        hasFired = true;

        OnChunkRendered?.Invoke(this);
    }
}
