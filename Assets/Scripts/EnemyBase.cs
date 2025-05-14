using UnityEngine;

public abstract class EnemyBase : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] protected float moveSpeed;
    [SerializeField] protected float attackMoveSpeed;
    [SerializeField] protected float attackRange;
    [SerializeField] protected float stopDistance;

    [Header("XP Drop")]
    [Tooltip("Quanto XP esse inimigo dá quando morre")]
    [SerializeField] protected int xpDrop = 10;

    [Header("Pooling")]
    public GameObject prefabRef;

    [Header("Áudio")]
    [SerializeField] protected AudioClip attackClip;
    [SerializeField] protected AudioClip deathClip;

    protected Rigidbody2D rb;
    protected Animator anim;
    protected Transform player;
    protected bool isAttacking;
    protected bool isDead;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    protected virtual void OnEnable()
    {
        isAttacking = false;
        isDead = false;
    }

    protected virtual void FixedUpdate()
    {
        if (player == null || isDead) return;

        Vector2 dir = (player.position - transform.position).normalized;
        float dist = Vector2.Distance(player.position, transform.position);

        HandleMovement(dir, dist);
        HandleAttack(dist);
    }

    protected virtual void HandleMovement(Vector2 dir, float dist)
    {
        if (isAttacking) return;
        if (dist > stopDistance)
            rb.MovePosition(rb.position + dir * moveSpeed * Time.fixedDeltaTime);
    }

    protected virtual void HandleAttack(float dist)
    {
        if (isAttacking) return;
        if (dist <= attackRange)
        {
            isAttacking = true;
            rb.linearVelocity = Vector2.zero;
            if (attackClip) AudioSource.PlayClipAtPoint(attackClip, transform.position);
            anim.SetTrigger("Attack");
        }
    }

    public virtual void AttackFinished()
    {
        // usado por alguns inimigos pra dar dano
        isAttacking = false;
    }

    public virtual void Kill()
    {
        if (isDead) return;
        isDead = true;
        DeathFinished();
    }

    public virtual void DeathFinished()
    {
        // primeiro, toca o som de morte
        if (deathClip) AudioSource.PlayClipAtPoint(deathClip, transform.position);

        // **DROP DE XP AQUI**
        var px = FindObjectOfType<PlayerExperience>();
        if (px != null)
            px.AddXP(xpDrop);

        // depois, solta no pool ou destrói
        if (EnemyPool.Instance)
            EnemyPool.Instance.Release(gameObject, prefabRef);
        else
            Destroy(gameObject);
    }
}