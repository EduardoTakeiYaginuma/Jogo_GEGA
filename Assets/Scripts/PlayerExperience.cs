using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class PlayerExperience : MonoBehaviour
{
    public int baseXP;
    public int xpIncrement;
    public int currentLevel;
    public int currentXP;
    public UnityEvent onLevelUp;
    private XpBar xpBar;

    float xpMultiplier = 1f;
    Coroutine xpBoostRoutine;
    bool waitingUpgrade = false;

    public int XPToNextLevel => baseXP + (currentLevel - 1) * xpIncrement;

    void Start()
    {
        xpBar = Object.FindFirstObjectByType<XpBar>();
        xpBar?.SetXp(currentXP, XPToNextLevel, currentLevel);
    }

    public void AddXP(int amount)
    {
        if (waitingUpgrade) return;

        int gained = Mathf.RoundToInt(amount * xpMultiplier);
        int need   = XPToNextLevel - currentXP;
        if (gained > need) gained = need;

        currentXP += gained;
        Debug.Log($"[XP] Ganhou {gained}. Agora {currentXP}/{XPToNextLevel} no level {currentLevel}");

        if (currentXP >= XPToNextLevel)
        {
            currentXP = 0;
            currentLevel++;
            waitingUpgrade = true;
            onLevelUp.Invoke();
            Debug.Log($"[XP] Level UP para {currentLevel}. XP zerado.");
        }

        xpBar?.SetXp(currentXP, XPToNextLevel, currentLevel);
    }

    public void ActivateXPBoost(float multiplier, float duration)
    {
        if (xpBoostRoutine != null) StopCoroutine(xpBoostRoutine);
        xpBoostRoutine = StartCoroutine(XPBoostRoutine(multiplier, duration));
    }

    IEnumerator XPBoostRoutine(float multiplier, float duration)
    {
        xpMultiplier = multiplier;
        yield return new WaitForSeconds(duration);
        xpMultiplier = 1f;
    }

    public void ClearWaitingFlag() => waitingUpgrade = false;
}
