using UnityEngine;

/// Spawna pequenos orbs de XP dentro de bounds passados pelo
/// InfiniteMapGenerator, longe do player. Singleton para chamada fácil.
public class XPSpawnManager : MonoBehaviour
{
    public static XPSpawnManager Instance { get; private set; }

    [Header("Orb prefab + regras")]
    [SerializeField] GameObject xpOrbPrefab;
    [SerializeField] int   minOrbs = 2;
    [SerializeField] int   maxOrbs = 5;
    [SerializeField] float safeDistFromPlayer = 10f;

    Transform player;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    /// Chame isso logo após criar o chunk.
    public void SpawnOrbsInChunk(Bounds area, Transform parent)
    {
        if (!xpOrbPrefab || !player) return;

        int qty = Random.Range(minOrbs, maxOrbs + 1);

        for (int i = 0; i < qty; i++)
        {
            Vector2 pos;
            int tries = 0;
            do
            {
                pos = new Vector2(
                    Random.Range(area.min.x, area.max.x),
                    Random.Range(area.min.y, area.max.y)
                );
            } while (Vector2.Distance(pos, player.position) < safeDistFromPlayer && ++tries < 20);

            Instantiate(xpOrbPrefab,
                        new Vector3(pos.x, pos.y, 0),
                        Quaternion.identity,
                        parent);
        }
    }
}