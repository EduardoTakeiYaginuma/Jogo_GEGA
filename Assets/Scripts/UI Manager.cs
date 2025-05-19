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
    [SerializeField] private GameObject       levelUpPanel;
    [SerializeField] private HealthBar        healthBar;
    [SerializeField] private GameObject       playerAura;
    [SerializeField] private GameObject[]     levelUpButtons;   // 0=Cura,1=Aura,2=Vel+15%,3=XP×2,4=CD–25%,5=StaMax+25%,6=StaRegen+25%,7=Dano–20%

    /* ---------------- DEBUG LOOP ---------------- */
    const float DEBUG_INTERVAL = 1f;   // imprime a cada 1 s
    float debugTimer;
    /* -------------------------------------------- */

    void Awake()
    {
        levelUpPanel.SetActive(false);
    }

    void Update()
    {
        debugTimer += Time.unscaledDeltaTime;
        if (debugTimer >= DEBUG_INTERVAL)
        {
            PrintDebugStats();
            debugTimer = 0f;
        }
    }

    public void ShowLevelUpPanel()
    {
        levelUpPanel.SetActive(true);
        Time.timeScale         = 0f;
        playerMovement.enabled = false;

        int level   = playerExperience.currentLevel;
        int healIdx = 0, dmgIdx = 7;

        bool healMandatory = (level % 2 == 0 && level > 2);
        bool dmgMandatory  = (level % 3 == 0);

        var choices = new List<int>();
        if (healMandatory) choices.Add(healIdx);
        if (dmgMandatory)  choices.Add(dmgIdx);

        var pool = new List<int>();
        for (int i = 0; i < levelUpButtons.Length; i++)
            if (i != healIdx && i != dmgIdx)
                pool.Add(i);

        while (choices.Count < 3 && pool.Count > 0)
        {
            int r = Random.Range(0, pool.Count);
            choices.Add(pool[r]);
            pool.RemoveAt(r);
        }

        for (int i = 0; i < choices.Count; i++)
        {
            int idx = choices[i];
            var go  = Instantiate(levelUpButtons[idx], levelUpPanel.transform);

            var rt = go.GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(-600 + 600 * i, 190);

            int captured = idx;
            go.GetComponent<Button>()
              .onClick.AddListener(() => OnUpgradeSelected(captured));
        }
    }

    void OnUpgradeSelected(int idx)
    {
        switch (idx)
        {
            case 0: gameController.SetHealth(gameController.maxHealth);        break;
            case 1: playerAura.transform.localScale *= 1.10f;                  break;
            case 2:
                playerMovement.walkSpeed *= 1.15f;
                playerMovement.runSpeed  *= 1.15f;
                break;
            case 3: playerExperience.ActivateXPBoost(2f, 60f);                 break;
            case 4:
                playerMovement.attackCooldown *= 0.75f;
                playerMovement.ClampAttackCooldown();
                break;
            case 5: playerMovement.IncreaseMaxStamina(0.25f);                  break;
            case 6: playerMovement.BoostStaminaRegen(0.25f);                   break;
            case 7: gameController.ReduceDamageTaken(0.20f);                   break;
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

    /* ================= DEBUG ==================== */
    void PrintDebugStats()
    {
        float regenRate = playerMovement.maxStamina / playerMovement.fullRecoveryTime;
        Debug.Log(
            $"[STATS] walk={playerMovement.walkSpeed:F2}, run={playerMovement.runSpeed:F2}, " +
            $"atkCD={playerMovement.attackCooldown:F2}s, maxSta={playerMovement.maxStamina:F2}, " +
            $"regenRate={regenRate:F2}/s (fullRec={playerMovement.fullRecoveryTime:F2}s)"
        );
    }
}
