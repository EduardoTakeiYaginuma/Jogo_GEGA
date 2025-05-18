using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public EnemyEntry[] enemies;

    [System.Serializable]
    public struct EnemyEntry
    {
        public GameObject prefab;
        [Min(0f)] public float weight;
    }

    [Header("Spawn Settings")]
    public float spawnRadius;
    public float startInterval;
    public float minInterval;
    public float decreasePerMinute;

    [Header("Burst Settings")]
    public int initialBurstCount;
    public float burstInterval;

    private float currentInterval;
    private float elapsedTime;

    void Start()
    {
        currentInterval = startInterval;
        elapsedTime = 0f;
        StartCoroutine(StartupSequence());
    }

    IEnumerator StartupSequence()
    {
        for (int i = 0; i < initialBurstCount; i++)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(burstInterval);
        }
        StartCoroutine(SpawnLoop());
    }

    IEnumerator SpawnLoop()
    {
        while (true)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(currentInterval);

            elapsedTime += currentInterval;
            float minutes = elapsedTime / 60f;
            float target = startInterval - decreasePerMinute * minutes;
            currentInterval = Mathf.Max(minInterval, target);
        }
    }

    void SpawnEnemy()
    {
        if (enemies == null || enemies.Length == 0) return;

        float totalWeight = 0f;
        foreach (var e in enemies)
            totalWeight += Mathf.Max(0f, e.weight);
        if (totalWeight <= 0f) return;

        float pick = Random.value * totalWeight;
        float acc = 0f;
        GameObject chosen = enemies[0].prefab;
        foreach (var e in enemies)
        {
            acc += Mathf.Max(0f, e.weight);
            if (pick <= acc)
            {
                chosen = e.prefab;
                break;
            }
        }

        Vector2 dir = Random.insideUnitCircle.normalized;
        Vector3 pos = player.position + (Vector3)dir * spawnRadius;
        EnemyPool.Instance.Get(chosen, pos, Quaternion.identity);
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (!player) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(player.position, spawnRadius);
    }
#endif
}
