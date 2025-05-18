using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Velocidades")]
    public float walkSpeed;
    public float runSpeed;

    [Header("Stamina")]
    public float maxStamina;
    public float fullRecoveryTime;
    float currentStamina;
    float staminaRegenRate;

    public float CurrentStamina => currentStamina;

    Rigidbody2D rb;
    Animator    anim;

    // hashes de parâmetro
    static readonly int H      = Animator.StringToHash("Horizontal");
    static readonly int V      = Animator.StringToHash("Vertical");
    static readonly int ISRUN  = Animator.StringToHash("IsRunning");
    static readonly int ISMOVE = Animator.StringToHash("IsMoving");
    static readonly int TR_ATK = Animator.StringToHash("Attack");

    bool    isAttacking;
    bool    isRunning;
    Vector2 moveInput;

    void Awake()
    {
        rb   = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        currentStamina   = maxStamina;
        staminaRegenRate = maxStamina / fullRecoveryTime;
    }

    void Update()
    {
        moveInput = new Vector2(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical")
        ).normalized;

        bool wantsToRun = Input.GetKey(KeyCode.LeftShift) && moveInput != Vector2.zero;
        isRunning = wantsToRun && currentStamina > 0f;

        anim.SetFloat(H, moveInput.x);
        anim.SetFloat(V, moveInput.y);
        anim.SetBool(ISRUN,  isRunning);
        anim.SetBool(ISMOVE, !isAttacking && moveInput != Vector2.zero);

        if (isRunning)
        {
            currentStamina -= Time.deltaTime;
            currentStamina = Mathf.Max(currentStamina, 0f);
        }
        else if (!Input.GetKey(KeyCode.LeftShift) && currentStamina < maxStamina)
        {
            currentStamina += staminaRegenRate * Time.deltaTime;
            currentStamina = Mathf.Min(currentStamina, maxStamina);
        }

        if (Input.GetKeyDown(KeyCode.Space) && !isAttacking)
            StartCoroutine(AttackRoutine());
    }

    void FixedUpdate()
    {
        float speed = isRunning ? runSpeed : walkSpeed;
        rb.linearVelocity = isAttacking ? Vector2.zero : moveInput * speed;
    }

    System.Collections.IEnumerator AttackRoutine()
    {
        isAttacking = true;
        anim.ResetTrigger(TR_ATK);
        anim.SetTrigger(TR_ATK);
        float clipLen = anim.GetCurrentAnimatorClipInfo(0)[0].clip.length;
        yield return new WaitForSeconds(clipLen);
        isAttacking = false;
    }
}
