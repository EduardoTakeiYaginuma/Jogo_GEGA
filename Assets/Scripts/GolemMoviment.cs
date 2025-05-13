using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public class GolemMoviment : MonoBehaviour
{
    [Header("Movimento")]
    [SerializeField] float moveSpeed = 2f;  // Golem pode ser mais lento que o vampiro
    [SerializeField] float minDistance = 0.8f;

    [Header("Pooling")]
    [Tooltip("Arraste aqui o próprio prefab do Golem")]
    [SerializeField] GameObject prefabRef;

    [Header("Áudio")]
    [Tooltip("Clip tocado no início do ataque")]
    [SerializeField] AudioClip attackClip;

    Rigidbody2D rb;
    Animator anim;
    Transform player;
    bool isAttacking;

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
        anim.ResetTrigger("Attack");
    }

    void FixedUpdate()
    {
        if (isAttacking || player == null) return;

        Vector2 dir = player.position - transform.position;

        if (dir.sqrMagnitude < minDistance * minDistance)
        {
            StartAttack(dir);
            return;
        }

        Vector2 move = dir.normalized * moveSpeed;

        // Corrigido para rb.velocity
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
}
