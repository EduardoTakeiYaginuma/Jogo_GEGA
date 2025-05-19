/*  UIManager.cs  */
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] PlayerMovement   playerMovement;
    [SerializeField] PlayerExperience playerExperience;
    [SerializeField] GameObject       levelUpPanel;
    [SerializeField] HealthBar        healthBar;
    [SerializeField] GameObject       playerAura;
    [SerializeField] GameObject[]     levelUpButtons;   // 0 Cure,1 Aura,2 Speed,3 XP×2,4 CD,5 StaminaMax,6 StaminaRegen
    [SerializeField] GameController   gameController;

    public void ShowLevelUpPanel()
    {
        levelUpPanel.SetActive(true);
        Time.timeScale         = 0f;
        playerMovement.enabled = false;

        int level = playerExperience.currentLevel;
        bool mustHeal = (level % 3 == 0);
        int healIdx   = 0;

        var pool = new List<int>();
        for (int i = 0; i < levelUpButtons.Length; i++) pool.Add(i);
        pool.Remove(healIdx);

        var choices = new List<int>();
        if (mustHeal) choices.Add(healIdx);

        while (choices.Count < 3)
        {
            int r = Random.Range(0, pool.Count);
            choices.Add(pool[r]);
            pool.RemoveAt(r);
        }

        for (int i = 0; i < 3; i++)
        {
            int idx = choices[i];
            var go  = Instantiate(levelUpButtons[idx], levelUpPanel.transform);
            go.GetComponent<RectTransform>().anchoredPosition = new Vector2(-600 + 600 * i, 190);

            int captured = idx;
            go.GetComponent<Button>().onClick.AddListener(() => OnUpgradeSelected(captured));
        }
    }

    void OnUpgradeSelected(int idx)
    {
        switch (idx)
        {
            case 0:                                   // Cura total
                gameController.SetHealth(gameController.maxHealth);
                break;
            case 1:                                   // Aura +10 %
                playerAura.transform.localScale *= 1.10f;
                break;
            case 2:                                   // Velocidade +10 %
                playerMovement.walkSpeed *= 1.10f;
                playerMovement.runSpeed  *= 1.10f;
                break;
            case 3:                                   // XP×2 por 1 min
                playerExperience.ActivateXPBoost(2f, 60f);
                break;
            case 4:                                   // Cooldown –10 %
                playerMovement.attackCooldown *= 0.90f;
                playerMovement.ClampAttackCooldown();   // aplica piso
                break;
            case 5:                                   // Stamina Máx +10 %
                playerMovement.IncreaseMaxStamina(0.10f);
                break;
            case 6:                                   // Stamina Regen +10 %
                playerMovement.BoostStaminaRegen(0.10f);
                break;
        }
        HideLevelUpPanel();
    }

    void HideLevelUpPanel()
    {
        for (int i = levelUpPanel.transform.childCount - 1; i >= 0; i--)
            Destroy(levelUpPanel.transform.GetChild(i).gameObject);

        levelUpPanel.SetActive(false);
        Time.timeScale         = 1f;
        playerMovement.enabled = true;
        playerExperience.ClearWaitingFlag();
    }
}
