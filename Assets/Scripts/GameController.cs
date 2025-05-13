using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;
    public HealthBar healthBar;

    private Animator animator;
    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
        animator = GetComponent<Animator>();
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;
        currentHealth -= damage;
        healthBar.SetHealth(currentHealth);
        if (currentHealth <= 0) Die();
    }

    void Die()
    {
        isDead = true;
        animator.SetTrigger("Death");
        var movement = GetComponent<PlayerMovement>();
        if (movement != null) movement.enabled = false;
        Invoke(nameof(GoToMenu), 2f);
    }

    void GoToMenu() => SceneManager.LoadScene(1);
}