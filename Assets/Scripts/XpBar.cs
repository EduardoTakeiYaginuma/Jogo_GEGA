using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class XpBar : MonoBehaviour
{
    [Header("Referências da UI")]
    public Slider xpSlider;
    public TMP_Text xpText;     // Mostra "XP: atual / necessário"
    public TMP_Text levelText;  // Mostra o número do level dentro do escudo

    public void SetXp(int current, int max, int level)
    {
        xpSlider.maxValue = max;
        xpSlider.value = current;

        if (xpText != null)
            xpText.text = $"{level}";

        if (levelText != null)
            levelText.text = level.ToString();
    }
}
