using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private PlayerMovement   playerMovement;
    [SerializeField] private PlayerExperience playerExperience;
    [SerializeField] private GameObject       levelUpPanel;
    [SerializeField] private HealthBar        healthBar;
    [SerializeField] private GameObject       playerAura;
    [SerializeField] private GameObject[]     levelUpButtons; // 0=Cura,1=Aura,2=Speed,3=XP,4=CD
    [SerializeField] private GameController   gameController;

    public void ShowLevelUpPanel()
    {
        levelUpPanel.SetActive(true);
        Time.timeScale = 0f;
        playerMovement.enabled = false;

        int lvl = playerExperience.currentLevel;
        bool mustHeal = (lvl % 3 == 0);
        int healIndex = 0;

        var pool = new List<int>();
        for (int i = 0; i < levelUpButtons.Length; i++)
            pool.Add(i);

        // nunca deixe o healIndex no pool por padrão
        pool.Remove(healIndex);

        var choices = new List<int>();
        if (mustHeal)
        {
            choices.Add(healIndex);
        }

        // sorteia até ter 3 opções
        while (choices.Count < 3)
        {
            int r = Random.Range(0, pool.Count);
            choices.Add(pool[r]);
            pool.RemoveAt(r);
        }

        // instancia e registra listener corretamente
        for (int i = 0; i < 3; i++)
        {
            int idx = choices[i];
            var btnGO = Instantiate(levelUpButtons[idx], levelUpPanel.transform);
            var rect  = btnGO.GetComponent<RectTransform>();
            rect.anchoredPosition = new Vector2(-600 + 600 * i, 190);

            var btn = btnGO.GetComponent<Button>();
            int captured = idx; // captura o valor para o closure
            btn.onClick.AddListener(() => OnUpgradeSelected(captured));
        }
    }

    private void OnUpgradeSelected(int idx)
    {
        switch (idx)
        {
            case 0: // cura total
                UpdateHealthBar(gameController.maxHealth);
                break;
            case 1: // aura +10%
                playerAura.transform.localScale *= 1.10f;
                break;
            case 2: // speed +10%
                playerMovement.walkSpeed *= 1.10f;
                playerMovement.runSpeed  *= 1.10f;
                break;
            case 3: // XP×2 por 1 min
                playerExperience.ActivateXPBoost(2f, 60f);
                break;
            case 4: // cooldown –10%
                playerMovement.attackCooldown *= 0.90f;
                break;
        }
        HideLevelUpPanel();
    }

    public void UpdateHealthBar(int newHealth)
    {
        gameController.SetHealth(newHealth);
    }

    public void IncreasePlayerAura(float pct)
    {
        playerAura.transform.localScale *= 1f + pct;
    }

    public void IncreasePlayerSpeed(float pct)
    {
        playerMovement.walkSpeed *= 1f + pct;
        playerMovement.runSpeed  *= 1f + pct;
    }

    private void HideLevelUpPanel()
    {
        for (int i = levelUpPanel.transform.childCount - 1; i >= 0; i--)
            Destroy(levelUpPanel.transform.GetChild(i).gameObject);

        levelUpPanel.SetActive(false);
        Time.timeScale = 1f;
        playerMovement.enabled = true;

        playerExperience.ClearWaitingFlag();   // libera novos ganhos de XP
    }
}
