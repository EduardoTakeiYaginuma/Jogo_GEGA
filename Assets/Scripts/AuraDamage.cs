using UnityEngine;

public class AuraDamage: MonoBehaviour
{
    [SerializeField] LayerMask enemyMask;   // marque a layer dos inimigos

    void OnTriggerEnter2D(Collider2D col)
    {
        // só reage se o collider estiver na layer de inimigos
        if (((1 << col.gameObject.layer) & enemyMask) == 0) return;

        // tenta encontrar GhostMovement e matar instantaneamente
        var ghost = col.GetComponent<GhostMovement>();
        if (ghost != null)
        {
            ghost.Kill();          // hit‑kill → volta pro pool sem travar
        }
        else
        {
            // fallback: se for outro inimigo sem pooling
            Destroy(col.gameObject);
        }
    }
}