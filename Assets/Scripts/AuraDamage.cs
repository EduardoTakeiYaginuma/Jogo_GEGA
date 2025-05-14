using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
public class AuraDamage : MonoBehaviour
{
    [SerializeField] LayerMask enemyMask;
    [SerializeField] float tickCooldown = 1f;
    [SerializeField] SpriteRenderer auraSprite;
    [SerializeField] float flashTime = 0.08f;
    [SerializeField][Range(0f,1f)] float cooldownAlpha = 0.3f;
    [SerializeField] GameObject hitVFX;

    float nextHitAllowed;

    void Awake()
    {
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;
        var playerCol = transform.root.GetComponent<Collider2D>();
        if (playerCol) Physics2D.IgnoreCollision(col, playerCol);
        auraSprite.color = Color.white;
    }

    void Update()
    {
        auraSprite.color = Time.time < nextHitAllowed
            ? new Color(1f,1f,1f,cooldownAlpha)
            : Color.white;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (Time.time < nextHitAllowed) return;
        if (((1 << other.gameObject.layer) & enemyMask) == 0) return;
        if (other.TryGetComponent<EnemyBase>(out var enemy))
        {
            enemy.Kill();
            enemy.DeathFinished();
            var px = FindObjectOfType<PlayerExperience>();
            if (px != null) px.AddXP(enemy.XPDrop);
            nextHitAllowed = Time.time + tickCooldown;
            StartCoroutine(FlashAura());
            if (hitVFX) Instantiate(hitVFX, other.transform.position, Quaternion.identity);
        }
    }

    IEnumerator FlashAura()
    {
        auraSprite.color = Color.white;
        yield return new WaitForSeconds(flashTime);
        auraSprite.color = Time.time < nextHitAllowed
            ? new Color(1f,1f,1f,cooldownAlpha)
            : Color.white;
    }
}