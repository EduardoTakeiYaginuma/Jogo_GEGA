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
    [SerializeField] GameObject[]     levelUpButtons;   // 0 Cure,1 Aura,2 Speed,3 XP×2,4 CD,5 StaminaMax,6 StaminaRegen,7 Dmg–
    [SerializeField] GameController   gameController;

    /* ===================================================================== */

    public void ShowLevelUpPanel()
    {
        levelUpPanel.SetActive(true);
        Time.timeScale         = 0f;
        playerMovement.enabled = false;

        int level   = playerExperience.currentLevel;
        int healIdx = 0;
        int dmgIdx  = 7;

        bool healMandatory = (level % 2 == 0 && level > 2);  // 4,6,8,…
        bool dmgMandatory  = (level % 3 == 0);               // 3,6,9,…

        var choices = new List<int>();

        /* ----------------- 1) adiciona obrigatórios ------------------ */
        if (healMandatory) choices.Add(healIdx);
        if (dmgMandatory)  choices.Add(dmgIdx);

        /* ----------------- 2) monta pool sem heal/dmg ---------------- */
        var pool = new List<int>();
        for (int i = 0; i < levelUpButtons.Length; i++)
        {
            if (i == healIdx || i == dmgIdx) continue;   // nunca sorteia
            pool.Add(i);
        }

        /* ----------------- 3) completa até ter 3 opções -------------- */
        while (choices.Count < 3 && pool.Count > 0)
        {
            int r = Random.Range(0, pool.Count);
            choices.Add(pool[r]);
            pool.RemoveAt(r);
        }

        /* ----------------- 4) instância botões ----------------------- */
        for (int i = 0; i < 3; i++)
        {
            int idx = choices[i];
            var go  = Instantiate(levelUpButtons[idx], levelUpPanel.transform);
            go.GetComponent<RectTransform>().anchoredPosition =
                new Vector2(-600 + 600 * i, 190);

            int captured = idx;
            go.GetComponent<Button>()
              .onClick.AddListener(() => OnUpgradeSelected(captured));
        }
    }

    /* -------------------- resto do script permanece ----------------- */
    void OnUpgradeSelected(int idx)
    {
        switch (idx)
        {
            case 0: gameController.SetHealth(gameController.maxHealth);        break; // Cura
            case 1: playerAura.transform.localScale *= 1.10f;                  break; // Aura
            case 2: playerMovement.walkSpeed *= 1.10f;                         // Veloc.
                    playerMovement.runSpeed  *= 1.10f;                         break;
            case 3: playerExperience.ActivateXPBoost(2f, 60f);                 break; // XP×2
            case 4: playerMovement.attackCooldown *= 0.90f;                    // CD –
                    playerMovement.ClampAttackCooldown();                       break;
            case 5: playerMovement.IncreaseMaxStamina(0.10f);                  break; // Stamina+
            case 6: playerMovement.BoostStaminaRegen(0.10f);                   break; // Regen+
            case 7: gameController.ReduceDamageTaken(0.10f);                   break; // Dano –
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