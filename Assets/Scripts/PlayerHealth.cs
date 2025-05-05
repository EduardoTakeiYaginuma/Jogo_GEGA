using UnityEngine;
using UnityEngine.InputSystem;   // ← para detectar a tecla K
using UnityEngine.UI;            // Slider
using TMPro;                     // TextMeshPro (remova se não usar)

public class PlayerHealth : MonoBehaviour
{
    /* ----------- CONFIGURAÇÃO DE VIDA ----------- */
    [Header("Vida")]
    [SerializeField] private int maxHP = 100;
    [SerializeField] private int debugDamage = 25;   // quanto tira ao apertar K
    private int currentHP;

    /* --------------- REFERÊNCIAS ---------------- */
    [Header("Referências UI")]
    [SerializeField] private Slider healthBar;
    [SerializeField] private TextMeshProUGUI hpText; // opcional

    private Animator        anim;
    private Rigidbody2D     rb;
    private PlayerMovement  movement;

    private static readonly int TR_DIE    = Animator.StringToHash("Die");
    private static readonly int DEATH_DIR = Animator.StringToHash("DeathDir");

    /* --------------- LIFE CYCLE ----------------- */
    private void Awake()
    {
        currentHP = maxHP;

        anim     = GetComponent<Animator>();
        rb       = GetComponent<Rigidbody2D>();
        movement = GetComponent<PlayerMovement>();
    }

    private void Start() => UpdateUI();

    private void Update()
    {
        /* ----------- DEBUG DAMAGE (K) ------------ */
        if (Keyboard.current.kKey.wasPressedThisFrame)
            TakeDamage(debugDamage, Vector2.down);   // direção arbitrária
    }

    /* --------------- API PÚBLICA ---------------- */
    public void TakeDamage(int dmg, Vector2 hitDir)
    {
        if (currentHP <= 0) return;

        currentHP = Mathf.Max(currentHP - dmg, 0);
        UpdateUI();

        if (currentHP == 0)
            Die(hitDir);
    }

    /* --------------- PRIVADO ------------------- */
    private void UpdateUI()
    {
        if (healthBar)
        {
            healthBar.maxValue = maxHP;
            healthBar.value    = currentHP;
        }

        if (hpText)
            hpText.text = $"{currentHP}/{maxHP}";
    }

    private void Die(Vector2 hitDir)
    {
        movement.enabled = false;
        rb.linearVelocity      = Vector2.zero;   // ← corrigido

        /* Define direção se seu Blend Tree de morte usa 1D */
        float dir = Mathf.Abs(hitDir.x) > Mathf.Abs(hitDir.y)
                    ? (hitDir.x < 0 ? 1f : 2f)      // Left / Right
                    : (hitDir.y < 0 ? 3f : 0f);     // Back / Front
        anim.SetFloat(DEATH_DIR, dir);

        anim.SetTrigger(TR_DIE);
        Destroy(gameObject, 3f);
    }
}
