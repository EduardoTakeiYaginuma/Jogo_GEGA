using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Velocidades")]
    [SerializeField] private float walkSpeed = 4f;
    [SerializeField] private float runSpeed  = 7f;

    private Rigidbody2D rb;
    private Animator animator;

    // Hashes → evita alocar strings por frame
    private static readonly int HORIZONTAL = Animator.StringToHash("Horizontal");
    private static readonly int VERTICAL   = Animator.StringToHash("Vertical");
    private static readonly int IS_RUNNING = Animator.StringToHash("IsRunning");
    private static readonly int TR_ATTACK  = Animator.StringToHash("Attack");     // ⬅ novo

    bool isMoving; // ⬅ novo

    private void Awake()
    {
        rb       = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        /* ---------- 1. INPUT ---------- */
        Vector2 move = InputManager.Movement;
        bool    run  = InputManager.Running  && move != Vector2.zero;
        bool    atk  = InputManager.Attacking;               // ⬅ novo

        /* ---------- 2. FÍSICA ---------- */
        float speed = run ? runSpeed : walkSpeed;
        rb.linearVelocity = move * speed; 
        
        if (rb.linearVelocity != Vector2.zero)
            isMoving = true; // ⬅ novo
        else
            isMoving = false; // ⬅ novo

        /* ---------- 3. ANIMAÇÃO ---------- */
        animator.SetFloat(HORIZONTAL, move.x);
        animator.SetFloat(VERTICAL,   move.y);
        animator.SetBool (IS_RUNNING, run);
        animator.SetBool ("IsMoving",  isMoving); // ⬅ novo

        if (atk)
            animator.SetTrigger(TR_ATTACK);                  // dispara Attack
    }
}
