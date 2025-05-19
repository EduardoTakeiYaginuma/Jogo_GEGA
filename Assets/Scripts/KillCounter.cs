using UnityEngine;
using TMPro;   // TextMeshPro

public class KillCounter : MonoBehaviour
{
    public static KillCounter Instance { get; private set; }

    [SerializeField] TextMeshProUGUI counterText;

    int kills = 0;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (counterText == null)
            counterText = GetComponentInChildren<TextMeshProUGUI>();

        UpdateUI();
    }

    public void RegisterKill()
    {
        kills++;
        UpdateUI();
    }

    void UpdateUI()
    {
        if (counterText)
            counterText.text = kills.ToString();
    }
}