using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Referências")]
    public Transform player;

    // ——— inimigos + chance relativa ———
    [System.Serializable]
    public struct EnemyEntry
    {
        public GameObject prefab;      // arraste o prefab
        [Min(0f)] public float weight; // peso/chance (0 = nunca spawna)
    }

    [Tooltip("Liste aqui todos os inimigos e o peso de cada um")]
    public EnemyEntry[] enemies;

    // ——— parâmetros de spawn ———
    [Header("Spawn")]
    public float spawnRadius   = 12f;
    public float startInterval = 2f;
    public float minInterval   = 0.3f;
    public float decreaseStep  = 20f;
    public float intervalMult  = 0.9f;

    float currentInterval;
    float accelTimer;

    void Start()
    {
        currentInterval = startInterval;
        StartCoroutine(SpawnLoop());
    }

    IEnumerator SpawnLoop()
    {
        while (true)
        {
            SpawnEnemy();

            yield return new WaitForSeconds(currentInterval);
            accelTimer += currentInterval;

            if (accelTimer >= decreaseStep && currentInterval > minInterval)
            {
                currentInterval = Mathf.Max(minInterval, currentInterval * intervalMult);
                accelTimer = 0f;
            }
        }
    }

    void SpawnEnemy()
    {
        if (enemies == null || enemies.Length == 0) return;

        // 1) soma total dos pesos
        float total = 0f;
        foreach (var e in enemies)
            total += Mathf.Max(0f, e.weight);

        if (total <= 0f) return; // nenhum peso > 0

        // 2) sorteia número entre 0 e total
        float pick = Random.value * total;

        // 3) encontra quem caiu
        GameObject prefabEscolhido = enemies[0].prefab; // fallback
        float acumulado = 0f;
        foreach (var e in enemies)
        {
            acumulado += Mathf.Max(0f, e.weight);
            if (pick <= acumulado)
            {
                prefabEscolhido = e.prefab;
                break;
            }
        }

        // instancia / pega do pool
        Vector2 dir = Random.insideUnitCircle.normalized;
        Vector3 pos = player.position + (Vector3)dir * spawnRadius;
        EnemyPool.Instance.Get(prefabEscolhido, pos, Quaternion.identity);
    }

#if UNITY_EDITOR   // gizmo só no editor
    void OnDrawGizmosSelected()
    {
        if (!player) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(player.position, spawnRadius);
    }
#endif
}