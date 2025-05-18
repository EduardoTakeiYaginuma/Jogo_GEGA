using UnityEngine;
using UnityEngine.UI;

public class StaminaBar : MonoBehaviour
{
    [Tooltip("Arraste aqui seu Player (com PlayerMovement)")]
    public PlayerMovement player;
    [Tooltip("Arraste aqui o Slider")]
    public Slider slider;

    void Start()
    {
        slider.minValue = 0f;
        slider.maxValue = player.maxStamina;
    }

    void Update()
    {
        slider.value = player.CurrentStamina;
    }
}
