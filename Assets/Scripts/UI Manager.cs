// UIManager.cs
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerMovement   playerMovement;
    [SerializeField] private PlayerExperience playerExperience;
    [SerializeField] private GameController   gameController;

    [Header("Level-Up UI")]
    [SerializeField] private GameObject       levelUpPanel;      // O Panel filho do Canvas
    [SerializeField] private HealthBar        healthBar;
    [SerializeField] private GameObject       playerAura;
    [SerializeField] private GameObject[]     levelUpButtons;    // 0=Cura,1=Aura,2=Velocidade,3=XP×2,4=CD–,5=StaminaMax,6=StaminaRegen,7=Dano–

    private void Awake()
    {
        // Garante que o painel comece escondido
        levelUpPanel.SetActive(false);
    }

    /// <summary>
    /// Ativado quando o jogador sobe de nível.
    /// Exibe o painel, pausa o jogo e gera 3 botões de escolha.
    /// </summary>
    public void ShowLevelUpPanel()
    {
        // 1) Exibe painel e pausa o tempo
        levelUpPanel.SetActive(true);
        Time.timeScale         = 0f;
        playerMovement.enabled = false;

        // 2) Define índices fixos
        int level   = playerExperience.currentLevel;
        int healIdx = 0, dmgIdx = 7;

        bool healMandatory = (level % 2 == 0 && level > 2);  // nível 4,6,8…
        bool dmgMandatory  = (level % 3 == 0);               // nível 3,6,9…

        // 3) Monta lista de escolhas
        var choices = new List<int>();
        if (healMandatory) choices.Add(healIdx);
        if (dmgMandatory)  choices.Add(dmgIdx);

        // 4) Preenche pool com os demais índices
        var pool = new List<int>();
        for (int i = 0; i < levelUpButtons.Length; i++)
            if (i != healIdx && i != dmgIdx)
                pool.Add(i);

        // 5) Sorteia até ter 3 opções
        while (choices.Count < 3 && pool.Count > 0)
        {
            int r = Random.Range(0, pool.Count);
            choices.Add(pool[r]);
            pool.RemoveAt(r);
        }

        // 6) Instancia e posiciona cada botão dentro de levelUpPanel
        for (int i = 0; i < choices.Count; i++)
        {
            int idx = choices[i];
            var go  = Instantiate(levelUpButtons[idx], levelUpPanel.transform);

            // posicione conforme seu layout; este exemplo centraliza em X
            var rt = go.GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(-600 + 600 * i, 190);

            int captured = idx;
            go.GetComponent<Button>()
              .onClick.AddListener(() => OnUpgradeSelected(captured));
        }
    }

    /// <summary>
    /// Aplica o efeito do upgrade selecionado e fecha o painel.
    /// </summary>
    private void OnUpgradeSelected(int idx)
    {
        switch (idx)
        {
            case 0: gameController.SetHealth(gameController.maxHealth);        break;
            case 1: playerAura.transform.localScale *= 1.10f;                  break;
            case 2:
                playerMovement.walkSpeed *= 1.10f;
                playerMovement.runSpeed  *= 1.10f;
                break;
            case 3: playerExperience.ActivateXPBoost(2f, 60f);                 break;
            case 4:
                playerMovement.attackCooldown *= 0.90f;
                playerMovement.ClampAttackCooldown();
                break;
            case 5: playerMovement.IncreaseMaxStamina(0.10f);                  break;
            case 6: playerMovement.BoostStaminaRegen(0.10f);                   break;
            case 7: gameController.ReduceDamageTaken(0.10f);                   break;
        }

        HideLevelUpPanel();
    }

    /// <summary>
    /// Limpa os botões instanciados, oculta o painel e retoma o jogo.
    /// </summary>
    private void HideLevelUpPanel()
    {
        // Destrói todos os filhos do painel (os botões gerados)
        for (int i = levelUpPanel.transform.childCount - 1; i >= 0; i--)
            Destroy(levelUpPanel.transform.GetChild(i).gameObject);

        // Oculta painel e retoma o tempo
        levelUpPanel.SetActive(false);
        Time.timeScale         = 1f;
        playerMovement.enabled = true;
        playerExperience.ClearWaitingFlag();
    }
}
