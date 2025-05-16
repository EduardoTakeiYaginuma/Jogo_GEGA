using UnityEngine;
using System.Linq;   // para First()

[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Velocidades")]
    public float walkSpeed = 4f;
    public float runSpeed  = 7f;

    Rigidbody2D rb;
    Animator    anim;

    // hashes (evita alocar strings todo frame)
    static readonly int H      = Animator.StringToHash("Horizontal");
    static readonly int V      = Animator.StringToHash("Vertical");
    static readonly int ISRUN  = Animator.StringToHash("IsRunning");
    static readonly int ISMOVE = Animator.StringToHash("IsMoving");
    static readonly int TR_ATK = Animator.StringToHash("Attack");

    bool    isAttacking;
    Vector2 moveInput;   // armazenado para o FixedUpdate

    void Awake()
    {
        rb   = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        /* ---------- INPUT ---------- */
        moveInput = new Vector2(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical")
        ).normalized;

        bool running   = Input.GetKey(KeyCode.LeftShift) && moveInput != Vector2.zero;
        bool attackKey = Input.GetKeyDown(KeyCode.Space);

        /* ---------- ANIM PARÂMETROS ---------- */
        anim.SetFloat(H, moveInput.x);
        anim.SetFloat(V, moveInput.y);
        anim.SetBool (ISRUN,  running);
        anim.SetBool (ISMOVE, !isAttacking && moveInput != Vector2.zero);

        /* ---------- ATAQUE ---------- */
        if (attackKey && !isAttacking)
            StartCoroutine(AttackRoutine());
    }

    void FixedUpdate()
    {
        float speed = (Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed);
        rb.linearVelocity = isAttacking ? Vector2.zero : moveInput * speed;
    }

    /* ---------- COROUTINE: dispara clipe e libera ---------- */
    System.Collections.IEnumerator AttackRoutine()
    {
        isAttacking = true;

        anim.ResetTrigger(TR_ATK);   // limpa restos
        anim.SetTrigger(TR_ATK);     // dispara Attack

        // pega duração do clipe atual na Layer 0 (mais robusto que Event)
        float clipLen = anim.GetCurrentAnimatorClipInfo(0)[0].clip.length;

        yield return new WaitForSeconds(clipLen);
        isAttacking = false;
    }
}
