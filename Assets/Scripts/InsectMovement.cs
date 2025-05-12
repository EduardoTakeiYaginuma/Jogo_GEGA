using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public class InsectMovement : MonoBehaviour
{
    /* ----------------------- Inspector ----------------------- */
    [Header("Movimento")]
    [SerializeField] float moveSpeed   = 2f;   // velocidade constante

    [Header("Combate")]
    [SerializeField] float attackRange = 0.6f; // distância p/ iniciar ataque

    [Header("Pooling")]
    public GameObject prefabRef;

    [Header("Áudio")]
    [SerializeField] AudioClip attackClip;
    [SerializeField] AudioClip deathClip;

    /* ----------------------- Internals ----------------------- */
    Rigidbody2D rb;
    Animator    anim;
    Transform   player;
    bool        isAttacking;
    bool        isDead;

    /* -------------------- Unity Lifecycle -------------------- */
    void Awake()
    {
        rb     = GetComponent<Rigidbody2D>();
        anim   = GetComponentInChildren<Animator>();          // Animator no filho, se houver
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
        if (player == null) return;   // única saída antecipada agora

        /* 1. Calcula direção e distância ---------------------- */
        Vector2 toPlayer = player.position - transform.position;
        float   dist     = toPlayer.magnitude;
        Vector2 dir      = toPlayer.normalized;

        /* 2. Move sempre em direção ao player ---------------- */
        if (dist > 0.05f)                           // evita tremor quando colado
        {
            Vector2 nextPos = rb.position + dir * moveSpeed * Time.fixedDeltaTime;
            rb.MovePosition(nextPos);
        }

        /* 3. Atualiza animação de caminhada APENAS se livre --- */
        if (!isAttacking && !isDead)
        {
            anim.SetFloat("Speed", dist > attackRange ? moveSpeed : 0);
        }

        /* 4. Inicia ataque se dentro do alcance --------------- */
        if (!isAttacking && !isDead && dist <= attackRange)
        {
            rb.linearVelocity = Vector2.zero;       // pára momentaneamente
            anim.SetFloat("Speed", 0);
            StartAttack();
        }
    }

    /* ---------------------- Combate ------------------------ */
    void StartAttack()
    {
        if (isDead || isAttacking) return;          // garante disparo único
        isAttacking = true;

        if (attackClip)
            AudioSource.PlayClipAtPoint(attackClip, transform.position);

        anim.SetTrigger("Attack");
    }

    // Animation Event no último frame de Attack
    public void AttackFinished()
    {
        if (isDead) return;

        isAttacking = false;
        isDead      = true;
        anim.SetTrigger("Die");
    }

    // Animation Event no último frame de Die
    public void DeathFinished() => Despawn();

    /* ------------------------ Pool ------------------------- */
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