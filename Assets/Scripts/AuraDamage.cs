using UnityEngine;

public class AuraDamage : MonoBehaviour
{
    [SerializeField] LayerMask enemyMask;

    void OnTriggerEnter2D(Collider2D col)
    {
        if (((1 << col.gameObject.layer) & enemyMask) == 0) return;

        var enemy = col.GetComponent<EnemyBase>();
        if (enemy != null)
            enemy.Kill();
        else
            Destroy(col.gameObject);
    }
}