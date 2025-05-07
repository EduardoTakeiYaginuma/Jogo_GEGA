using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public class GhostMovement : MonoBehaviour
{
    [Header("Movimento")]
    [SerializeField] float moveSpeed   = 3f;
    [SerializeField] float minDistance = 0.6f;

    [Header("Pooling")]
    [Tooltip("Arraste aqui o próprio prefab do fantasma")]
    public GameObject prefabRef;

    [Header("Áudio")]
    [Tooltip("Clip tocado no início do ataque")]
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
        if (isAttacking || player == null) return;

        Vector2 dir = player.position - transform.position;

        if (dir.sqrMagnitude < minDistance * minDistance)
        {
            StartAttack(dir);
            return;
        }

        Vector2 move = dir.normalized * moveSpeed;
        rb.linearVelocity   = move;

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

    public void AttackFinished() => Kill();

    public void Kill()
    {
        EnemyPool.Instance.Release(gameObject, prefabRef);
    }
}