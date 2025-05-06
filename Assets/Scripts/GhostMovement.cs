using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public class GhostMovement : MonoBehaviour
{
    [Header("Movimento")]
    [SerializeField] float moveSpeed = 3f;
    [SerializeField] float minDistance = 0.6f;

    [Header("Combate")]
   

    Rigidbody2D rb;
    Animator anim;
    Transform player;
    bool isAttacking;

    void Awake()
    {
        rb     = GetComponent<Rigidbody2D>();
        anim   = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
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
        rb.linearVelocity   = move;   // <- corrigido

        anim.SetFloat("VelX", move.x);
        anim.SetFloat("VelY", move.y);
        anim.SetFloat("Speed", move.sqrMagnitude);
    }

    void StartAttack(Vector2 dir)
    {
        isAttacking  = true;
        rb.linearVelocity  = Vector2.zero;

        int d; // 0‑Down, 1‑Up, 2‑Left, 3‑Right
        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
            d = dir.x < 0 ? 2 : 3;
        else
            d = dir.y > 0 ? 1 : 0;

        anim.SetInteger("Dir", d);
        anim.SetTrigger("Attack");
       
    }

    // chamado via Animation Event no último frame
    public void AttackFinished()
    {
        Debug.Log("EVENTO AttackFinished recebido");
        Destroy(gameObject); // ou isAttacking = false;
    }
}
