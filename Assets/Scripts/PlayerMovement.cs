using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public class PlayerMovement : MonoBehaviour
{
    public float walkSpeed;
    public float runSpeed;
    public float maxStamina;
    public float fullRecoveryTime;
    public float attackCooldown = 1f;
    public Transform attackPoint;
    public float attackRadius = 0.5f;
    public LayerMask enemyLayers;

    float currentStamina;
    float staminaRegenRate;
    float nextAttackTime;

    public float CurrentStamina => currentStamina;

    Rigidbody2D rb;
    Animator anim;

    static readonly int H      = Animator.StringToHash("Horizontal");
    static readonly int V      = Animator.StringToHash("Vertical");
    static readonly int ISRUN  = Animator.StringToHash("IsRunning");
    static readonly int ISMOVE = Animator.StringToHash("IsMoving");
    static readonly int TR_ATK = Animator.StringToHash("Attack");

    bool isAttacking;
    bool isRunning;
    Vector2 moveInput;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        currentStamina = maxStamina;
        staminaRegenRate = maxStamina / fullRecoveryTime;
        nextAttackTime = 0f;
    }

    void Update()
    {
        moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
        bool wantsToRun = Input.GetKey(KeyCode.LeftShift) && moveInput != Vector2.zero;
        isRunning = wantsToRun && currentStamina > 0f;

        anim.SetFloat(H, moveInput.x);
        anim.SetFloat(V, moveInput.y);
        anim.SetBool(ISRUN, isRunning);
        anim.SetBool(ISMOVE, !isAttacking && moveInput != Vector2.zero);

        if (isRunning)
            currentStamina = Mathf.Max(0f, currentStamina - Time.deltaTime);
        else if (!Input.GetKey(KeyCode.LeftShift) && currentStamina < maxStamina)
            currentStamina = Mathf.Min(maxStamina, currentStamina + staminaRegenRate * Time.deltaTime);

        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)) 
            && !isAttacking 
            && Time.time >= nextAttackTime)
        {
            StartCoroutine(AttackRoutine());
        }
    }

    void FixedUpdate()
    {
        float speed = isRunning ? runSpeed : walkSpeed;
        rb.linearVelocity = isAttacking ? Vector2.zero : moveInput * speed;
    }

    IEnumerator AttackRoutine()
    {
        isAttacking = true;
        anim.ResetTrigger(TR_ATK);
        anim.SetTrigger(TR_ATK);

        float clipLen = anim.GetCurrentAnimatorClipInfo(0)[0].clip.length;
        float hitTime = 0.45f;
        yield return new WaitForSeconds(hitTime);

        Collider2D[] hits = Physics2D.OverlapCircleAll(attackPoint.position, attackRadius, enemyLayers);
        foreach (var col in hits)
        {
            if (col.TryGetComponent<EnemyBase>(out var enemy))
            {
                enemy.Kill();
                enemy.DeathFinished();
                var px = Object.FindFirstObjectByType<PlayerExperience>();
                if (px != null) px.AddXP(enemy.XPDrop);
            }
        }

        yield return new WaitForSeconds(clipLen - hitTime);
        isAttacking = false;
        nextAttackTime = Time.time + attackCooldown;
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }
}
