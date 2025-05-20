using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    [Header("Vida")]
    public int maxHealth = 100;
    public int currentHealth;
    public HealthBar healthBar;

    [Header("Dano recebido")]
    [Tooltip("1  = 100 % do dano; 0.9 = –10 % etc.")]
    public float damageMultiplier = 1f;    // reduzido pelos upgrades

    Animator   animator;
    Rigidbody2D rb;
    bool       isDead;

    /* ====================================================================== */

    void Start()
    {
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);

        animator = GetComponent<Animator>();
        rb       = GetComponent<Rigidbody2D>();
    }

    /* ============================  DANO  ================================== */

    public void TakeDamage(int rawDamage)
    {
        if (isDead) return;

        int finalDamage = Mathf.Max(1,
            Mathf.RoundToInt(rawDamage * damageMultiplier));   // aplica redução
        SetHealth(currentHealth - finalDamage);
    }

    /* chamado pelo upgrade “Pele de Aço” */
    public void ReduceDamageTaken(float pct)   // pct = 0.10 → –10 %
    {
        damageMultiplier *= 1f - pct;
        damageMultiplier = Mathf.Clamp(damageMultiplier, 0.1f, 1f); // evita zerar
    }

    /* ============================  VIDA  ================================== */

    public void SetHealth(int newHealth)
    {
        if (isDead) return;

        currentHealth = Mathf.Clamp(newHealth, 0, maxHealth);
        healthBar.SetHealth(currentHealth);

        if (currentHealth <= 0)
            Die();
    }

    /* ============================  MORTE  ================================= */

    void Die()
    {
        isDead = true;
        animator.SetTrigger("Death");

        if (rb != null)
            rb.linearVelocity = Vector2.zero;

        var movement = GetComponent<PlayerMovement>();
        if (movement != null)
            movement.enabled = false;

        Invoke(nameof(GoToGameover), 3f);
    }

    void GoToMenu() => SceneManager.LoadScene(0);
    
    void GoToGameover() => SceneManager.LoadScene(3);
}