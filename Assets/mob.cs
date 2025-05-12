using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public class mob : MonoBehaviour
{
    [Header("Movimento")]
    [SerializeField] float moveSpeed = 2f;          // velocidade constante

    [Header("Combate")]
    [SerializeField] float attackRange = 0.6f;      // distância para iniciar ataque

    [Header("Pooling")]
    public GameObject prefabRef;

    [Header("Áudio")]
    [SerializeField] AudioClip attackClip;
    [SerializeField] AudioClip deathClip;

    Rigidbody2D rb;
    Animator    anim;
    Transform   player;
    bool        isAttacking;
    bool        isDead;

    /* ------------------------- Unity Lifecycle ------------------------- */

    void Awake()
    {
        rb     = GetComponent<Rigidbody2D>();
        anim   = GetComponentInChildren<Animator>();   // pega Animator no filho, se houver
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    void OnEnable()
    {
        isAttacking = false;
        isDead      = false;

        anim.ResetTrigger("Attack");
        anim.ResetTrigger("Die");
        anim.SetFloat("Speed", 0);
    }

    void FixedUpdate()
    {
        if (player == null || isAttacking || isDead) return;

        /* 1. Caminha sempre em direção ao player ------------------------ */
        Vector2 dir  = (player.position - transform.position).normalized;
        float   dist = Vector2.Distance(player.position, transform.position);

        if (dist > attackRange)          // ainda está longe → continua andando
        {
            Vector2 nextPos = rb.position + dir * moveSpeed * Time.fixedDeltaTime;
            rb.MovePosition(nextPos);

            anim.SetFloat("Speed", moveSpeed);      // liga animação de caminhada
        }
        else                             // chegou perto → ataca
        {
            rb.linearVelocity = Vector2.zero;
            anim.SetFloat("Speed", 0);
            StartAttack();
        }
    }

    /* ----------------------- Estados de Combate ----------------------- */

    void StartAttack()
    {
        if (isDead || isAttacking) return;           // dispara apenas uma vez
        isAttacking = true;

        if (attackClip)
            AudioSource.PlayClipAtPoint(attackClip, transform.position);

        anim.SetTrigger("Attack");
    }

    // Animation Event no último frame do clipe Attack
    public void AttackFinished()
    {
        Debug.Log("<color=yellow>AttackFinished chamado!</color>");

        if (isDead) return;

        isAttacking = false;
        isDead      = true;

        Debug.Log("<color=cyan>Disparando Die trigger</color>");
        anim.SetTrigger("Die");
    }


    // Animation Event no último frame do clipe Die
    public void DeathFinished() => Despawn();

    /* ----------------------------- Pool ------------------------------ */

    void Despawn()
    {
        if (deathClip)
            AudioSource.PlayClipAtPoint(deathClip, transform.position);

        if (EnemyPool.Instance != null)
            EnemyPool.Instance.Release(gameObject, prefabRef);
        else
            Destroy(gameObject);
    }
}
