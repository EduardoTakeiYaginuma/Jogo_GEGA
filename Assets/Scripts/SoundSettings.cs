using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class SoundSettings : MonoBehaviour
{
    Slider slider;
    const string PrefKey = "MasterVolume";

    void Awake()
    {
        slider = GetComponent<Slider>();   // garante referência
    }

    void Start()
    {
        float saved = PlayerPrefs.GetFloat(PrefKey, 1f);
        slider.value = saved;
        ApplyVolume(saved);

        slider.onValueChanged.AddListener(ApplyVolume);
    }

    void ApplyVolume(float value)
    {
        AudioListener.volume = Mathf.Clamp01(value);
        PlayerPrefs.SetFloat(PrefKey, value);
    }
}