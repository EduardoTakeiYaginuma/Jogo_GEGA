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
        animator = GetComponent<Animator>(); // ou GetComponentInChildren<Animator>() se estiver num filho
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        healthBar.SetHealth(currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;

        // Dispara a animação de morte
        animator.SetTrigger("Death");

        // Opcional: desativar movimento
        var movement = GetComponent<PlayerMovement>();
        if (movement != null)
            movement.enabled = false;

        // Esperar a animação terminar
        float deathAnimDuration = 2f; // Ajuste para o tempo exato da animação
        Invoke(nameof(GoToMenu), deathAnimDuration);
    }

    void GoToMenu()
    {
        SceneManager.LoadScene(1); // ou SceneManager.LoadScene(1), como preferir
    }
}
