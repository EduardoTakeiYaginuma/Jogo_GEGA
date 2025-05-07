using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Referências")]
    public Transform player;
    public GameObject enemyPrefab;

    [Header("Spawn")]
    public float spawnRadius   = 12f;
    public float startInterval = 2f;
    public float minInterval   = 0.3f;
    public float decreaseStep  = 20f;   // a cada 20 s…
    public float intervalMult  = 0.9f;  // …intervalo × 0.9 (‑10 %)

    private float currentInterval;
    private float timer;

    void Start()
    {
        currentInterval = startInterval;
        StartCoroutine(SpawnLoop());
    }

    IEnumerator SpawnLoop()
    {
        float accelTimer = 0f;

        while (true)
        {
            SpawnEnemy();

            yield return new WaitForSeconds(currentInterval);
            accelTimer += currentInterval;

            if (accelTimer >= decreaseStep && currentInterval > minInterval)
            {
                currentInterval = Mathf.Max(minInterval, currentInterval * intervalMult);
                accelTimer = 0f;           // zera o cronômetro da próxima queda
            }
        }
    }

    void SpawnEnemy()
    {
        Vector2 dir = Random.insideUnitCircle.normalized;
        Vector3 pos = player.position + (Vector3)dir * spawnRadius;

        GameObject obj = EnemyPool.Instance.Get(enemyPrefab, pos, Quaternion.identity);
        // se precisar passar algo p/ o inimigo (ex.: prefabRef), faça aqui:
        // obj.GetComponent<EnemyHealth>().prefabRef = enemyPrefab;
    }

#if UNITY_EDITOR   // gizmo pra ver o raio na cena
    void OnDrawGizmosSelected()
    {
        if (!player) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(player.position, spawnRadius);
    }
#endif
}
