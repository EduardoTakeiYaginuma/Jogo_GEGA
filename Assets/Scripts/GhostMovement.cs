using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public class GhostMovement : MonoBehaviour
{
    [Header("Movimento")]
    [SerializeField] float moveSpeed        = 3f;    // patrulha
    [SerializeField] float attackMoveSpeed  = 1.5f;  // enquanto ataca
    [SerializeField] float minDistance      = 0.6f;  // range p/ começar ataque
    [SerializeField] float stopDistance     = 0.05f; // distância pra zerar vel

    [Header("Pooling")]
    public GameObject prefabRef;

    [Header("Áudio")]
    [SerializeField] AudioClip attackClip;

    Rigidbody2D rb;
    Animator    anim;
    Transform   player;
    bool        isAttacking;

    void Awake()
    {
        rb     = GetComponent<Rigidbody2D>();
        anim   = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    void OnEnable()
    {
        isAttacking = false;
        anim.ResetTrigger("Attack");
    }

    void FixedUpdate()
    {
        if (player == null) return;

        // ---------- Cálculo de direção / distância ----------
        Vector2 dir  = (Vector2)player.position - rb.position;
        float   dist = dir.magnitude;
        float   speed = isAttacking ? attackMoveSpeed : moveSpeed;

        // ---------- MOVIMENTAÇÃO SEM OVERSHOOT ----------
        if (dist > stopDistance)
        {
            Vector2 nextPos = Vector2.MoveTowards(
                                  rb.position,
                                  (Vector2)player.position,
                                  speed * Time.fixedDeltaTime);

            rb.MovePosition(nextPos);

            Vector2 vel = (nextPos - rb.position) / Time.fixedDeltaTime;
            anim.SetFloat("VelX", vel.x);
            anim.SetFloat("VelY", vel.y);
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
            anim.SetFloat("VelX", 0);
            anim.SetFloat("VelY", 0);
        }

        // ---------- CHECA SE COMEÇA ATAQUE ----------
        if (!isAttacking && dist < minDistance)
            StartAttack();
    }

    void StartAttack()
    {
        isAttacking = true;

        if (attackClip)
            AudioSource.PlayClipAtPoint(attackClip, transform.position, 1f);

        anim.SetTrigger("Attack");
    }

    // Animation Event no último frame do golpe
    public void AttackFinished() => Kill();

    public void Kill()
    {
        EnemyPool.Instance.Release(gameObject, prefabRef);
    }
}