using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public class GhostMovement : MonoBehaviour
{
    [Header("Movimento")]
    [SerializeField] float moveSpeed   = 3f;
    [SerializeField] float minDistance = 0.6f;

    [Header("Pooling")]
    [Tooltip("Arraste aqui o próprio prefab do fantasma")]
    [SerializeField] GameObject prefabRef;   // ← referência pro EnemyPool

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

    // sempre que o objeto sai do pool, resetamos estado básico
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
        rb.linearVelocity   = move;          // use velocity nas versões atuais

        anim.SetFloat("VelX", move.x);
        anim.SetFloat("VelY", move.y);
    }

    void StartAttack(Vector2 dir)
    {
        isAttacking = true;
        rb.linearVelocity = Vector2.zero;

        // (direção serve só pra anim mudar o frame)
        int d; // 0=D, 1=U, 2=L, 3=R
        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
            d = dir.x < 0 ? 2 : 3;
        else
            d = dir.y > 0 ? 1 : 0;

        anim.SetTrigger("Attack");
    }

    // chamado no último frame da animação via Animation Event
    public void AttackFinished()
    {
        // devolve o inimigo pro pool (adeus Destroy!)
        EnemyPool.Instance.Release(gameObject, prefabRef);
    }
}
