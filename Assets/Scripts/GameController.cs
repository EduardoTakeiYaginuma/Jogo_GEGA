using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;
    public HealthBar healthBar;

    private Animator animator;
    private bool isDead = false;
    private Rigidbody2D rb;

    void Start()
    {
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);

        animator = GetComponent<Animator>();
        rb       = GetComponent<Rigidbody2D>();
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;
        SetHealth(currentHealth - damage);
    }

    public void SetHealth(int newHealth)
    {
        if (isDead) return;
        currentHealth = Mathf.Clamp(newHealth, 0, maxHealth);
        healthBar.SetHealth(currentHealth);

        if (currentHealth <= 0)
            Die();
    }

    void Die()
    {
        isDead = true;
        animator.SetTrigger("Death");

        if (rb != null)
            rb.linearVelocity = Vector2.zero;

        var movement = GetComponent<PlayerMovement>();
        if (movement != null)
            movement.enabled = false;

        Invoke(nameof(GoToMenu), 3f);
    }

    void GoToMenu() => SceneManager.LoadScene(1);
}
