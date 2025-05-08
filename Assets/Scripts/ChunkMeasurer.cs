using UnityEngine;
using UnityEngine.Tilemaps;

[ExecuteAlways]
public class ChunkMeasurer : MonoBehaviour
{
    void Start()
    {
        var tm = GetComponentInChildren<Tilemap>();
        if (tm == null) return;
        
        var size = tm.cellBounds.size;
        var cell = tm.layoutGrid.cellSize;
        float width  = size.x * cell.x;
        float height = size.y * cell.y;

        Debug.Log($"📏 Chunk mede {width}×{height} unidades (tiles {size.x}×{size.y}, cell {cell.x}×{cell.y})");
        
        // só uma vez
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}