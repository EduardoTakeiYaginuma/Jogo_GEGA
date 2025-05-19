using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public class PlayerMovement : MonoBehaviour
{
    /* ------------ PUBLIC FIELDS (expostos no Inspector) ------------ */

    public float walkSpeed;
    public float runSpeed;

    [Header("Stamina (valores-base)")]
    public float maxStamina;
    public float fullRecoveryTime;

    [Header("Ataque")]
    public float attackCooldown;
    public Transform attackPoint;
    public float attackRadius;
    public LayerMask enemyLayers;

    [Header("Limites mínimos")]
    public float minAttackCooldown = 0.25f;   // não deixa spammar
    public float minFullRecovery   = 0.50f;   // evita recarga instantânea

    /* ------------ RUNTIME ------------ */

    float currentStamina;
    float staminaRegenRate;
    float nextAttackTime;

    /* escalas acumuladas pelos upgrades */
    float staminaScale = 1f;   // +10 % máx por carta
    float regenScale   = 1f;   // −10 % tempo por carta

    /* guarda valores-base originais */
    float baseMaxStamina;
    float baseRecovery;

    public float CurrentStamina => currentStamina;

    Rigidbody2D rb;
    Animator    anim;

    static readonly int H      = Animator.StringToHash("Horizontal");
    static readonly int V      = Animator.StringToHash("Vertical");
    static readonly int ISRUN  = Animator.StringToHash("IsRunning");
    static readonly int ISMOVE = Animator.StringToHash("IsMoving");
    static readonly int TR_ATK = Animator.StringToHash("Attack");

    bool    isAttacking;
    bool    isRunning;
    Vector2 moveInput;

    /* =============================================================== */

    void Awake()
    {
        rb   = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        baseMaxStamina = maxStamina;
        baseRecovery   = fullRecoveryTime;

        RecomputeStaminaStats();
        currentStamina = maxStamina;
    }

    void Update()
    {
        moveInput = new Vector2(Input.GetAxisRaw("Horizontal"),
                                Input.GetAxisRaw("Vertical")).normalized;

        bool wantsRun = Input.GetKey(KeyCode.LeftShift) && moveInput != Vector2.zero;
        isRunning     = wantsRun && currentStamina > 0f;

        anim.SetFloat(H, moveInput.x);
        anim.SetFloat(V, moveInput.y);
        anim.SetBool(ISRUN,  isRunning);
        anim.SetBool(ISMOVE, !isAttacking && moveInput != Vector2.zero);

        /* ---- consumo / regeneração da stamina ---- */
        if (isRunning)
        {
            float drain = Time.deltaTime / staminaScale;
            currentStamina = Mathf.Max(0f, currentStamina - drain);
        }
        else if (!Input.GetKey(KeyCode.LeftShift) && currentStamina < maxStamina)
        {
            currentStamina = Mathf.Min(maxStamina,
                                       currentStamina + staminaRegenRate * Time.deltaTime);
        }

        /* ---- ataque ---- */
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)) &&
            !isAttacking && Time.time >= nextAttackTime)
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

        Collider2D[] hits = Physics2D.OverlapCircleAll(attackPoint.position,
                                                       attackRadius,
                                                       enemyLayers);
        foreach (var col in hits)
            if (col.TryGetComponent<EnemyBase>(out var enemy))
            {
                enemy.Kill();
                enemy.DeathFinished();

                var px = Object.FindFirstObjectByType<PlayerExperience>();
                if (px != null) px.AddXP(enemy.XPDrop);

                KillCounter.Instance?.RegisterKill();   // +1 kill
            }

        yield return new WaitForSeconds(clipLen - hitTime);
        isAttacking    = false;
        nextAttackTime = Time.time + attackCooldown;
    }

    /* ===================  UPGRADES  =================== */

    public void IncreaseMaxStamina(float pct)   // +10 % máx
    {
        staminaScale *= 1f + pct;
        RecomputeStaminaStats();
    }

    public void BoostStaminaRegen(float pct)    // −10 % tempo
    {
        regenScale *= 1f - pct;
        RecomputeStaminaStats();
    }

    // chamado pelo UIManager após diminuir 10 % do CD
    public void ClampAttackCooldown()
    {
        attackCooldown = Mathf.Max(attackCooldown, minAttackCooldown);
    }

    void RecomputeStaminaStats()
    {
        maxStamina       = baseMaxStamina * staminaScale;
        fullRecoveryTime = Mathf.Max(baseRecovery * staminaScale * regenScale,
                                     minFullRecovery);
        staminaRegenRate = maxStamina / fullRecoveryTime;
        currentStamina   = Mathf.Min(currentStamina, maxStamina);
    }

    /* gizmo */
    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }
}