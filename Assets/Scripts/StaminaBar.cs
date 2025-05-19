using UnityEngine;
using UnityEngine.UI;

public class StaminaBar : MonoBehaviour
{
    [Tooltip("Arraste aqui seu PlayerMovement")]
    public PlayerMovement player;
    [Tooltip("Arraste aqui o Slider")]
    public Slider slider;

    void Start()
    {
        slider.minValue = 0f;
    }

    void Update()
    {
        slider.maxValue = player.maxStamina;      // mantém o novo máximo
        slider.value    = player.CurrentStamina;  // atualiza o valor
    }
}
