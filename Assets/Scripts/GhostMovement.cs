using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public class GhostMovement : MonoBehaviour
{
    [Header("Movimento")]
    [SerializeField] float moveSpeed   = 3f;
    [SerializeField] float minDistance = 0.6f;

    [Header("Pooling")]
    [Tooltip("Arraste aqui o próprio prefab do fantasma")]
    [SerializeField] GameObject prefabRef;

    [Header("Áudio")]
    [Tooltip("Clip tocado no início do ataque")]
    [SerializeField] AudioClip attackClip;

    Rigidbody2D rb;
    Animator    anim;
    Transform   player;
    bool        isAttacking;
    
    public GameController gameController;

    void Awake()
    {
        rb     = GetComponent<Rigidbody2D>();
        anim   = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    // reset sempre que sai do pool
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
#if UNITY_2022_3_OR_NEWER
        rb.linearVelocity = move;
#else
        rb.linearVelocity = move;
#endif
        anim.SetFloat("VelX", move.x);
        anim.SetFloat("VelY", move.y);
    }

    void StartAttack(Vector2 dir)
    {
        isAttacking = true;
        rb.linearVelocity = Vector2.zero;

        // --- toca o som num GameObject temporário, não corta ao desativar ---
        if (attackClip)
            AudioSource.PlayClipAtPoint(attackClip, transform.position, 1f);

        anim.SetTrigger("Attack");
    }

    // último frame da animação
    public void AttackFinished()
    {
        EnemyPool.Instance.Release(gameObject, prefabRef);
    }
}
