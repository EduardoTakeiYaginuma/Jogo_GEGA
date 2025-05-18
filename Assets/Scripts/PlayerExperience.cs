using UnityEngine;
using UnityEngine.Events;

public class PlayerExperience : MonoBehaviour
{
    public int baseXP;
    public int xpIncrement;
    public int currentLevel;
    public int currentXP;
    public UnityEvent onLevelUp;


    public int XPToNextLevel => CalculateXPForLevel(currentLevel);

    public int CalculateXPForLevel(int level)
    {
        return baseXP + (level - 1) * xpIncrement;
    }

    public void AddXP(int amount)
    {
        currentXP += amount;
        while (currentXP >= XPToNextLevel)
        {
            currentXP -= XPToNextLevel;
            currentLevel++;
            onLevelUp.Invoke();
            Debug.Log($"[XP] Level UP! Agora nível {currentLevel}");
            

        }
        Debug.Log($"[XP] XP atual: {currentXP}/{XPToNextLevel} no Level {currentLevel}");

    }



}