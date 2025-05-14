using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public class GolemMoviment : MonoBehaviour
{
    [Header("Movimento")]
    [SerializeField] float moveSpeed = 2f;
    [SerializeField] float minDistance = 0.8f;

    [Header("Pooling")]
    [SerializeField] GameObject prefabRef;

    [Header("Áudio")]
    [SerializeField] AudioClip attackClip;

    Rigidbody2D rb;
    Animator anim;
    Transform player;
    bool isAttacking;
    bool isDead;

    public GameController gameController;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    void OnEnable()
    {
        isAttacking = false;
        isDead = false;
        anim.ResetTrigger("Attack");
    }

    void FixedUpdate()
    {
        if (isAttacking || isDead || player == null) return;

        Vector2 dir = player.position - transform.position;

        if (dir.sqrMagnitude < minDistance * minDistance)
        {
            StartAttack(dir);
            return;
        }

        Vector2 move = dir.normalized * moveSpeed;
        rb.linearVelocity = move;
        anim.SetFloat("VelX", move.x);
        anim.SetFloat("VelY", move.y);
    }

    void StartAttack(Vector2 dir)
    {
        isAttacking = true;
        rb.linearVelocity = Vector2.zero;

        if (attackClip)
            AudioSource.PlayClipAtPoint(attackClip, transform.position, 1f);

        anim.SetTrigger("Attack");
    }

    public void AttackFinished()
    {
        EnemyPool.Instance.Release(gameObject, prefabRef);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (isDead) return;

        if (collision.CompareTag("Aura"))
        {
            // Tenta pegar o SpriteRenderer para verificar se a aura está "ativa"
            SpriteRenderer sr = collision.GetComponent<SpriteRenderer>();
            if (sr != null && sr.color.a > 0.5f)  // Aura "ativa" se alpha for alto
            {
                DieNow();
            }
        }
    }


    void DieNow()
    {
        isDead = true;
        rb.linearVelocity = Vector2.zero;
        EnemyPool.Instance.Release(gameObject, prefabRef); // ou Destroy(gameObject)
    }
}
