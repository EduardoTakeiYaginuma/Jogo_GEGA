using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundSettings : MonoBehaviour
{
    [SerializeField] private Slider     volumeSlider;
    [SerializeField] private AudioMixer masterMixer;

    private const string PrefKey = "MasterVolume";

    private void Start()
    {
        // 1f = 100% de início
        float saved = PlayerPrefs.GetFloat(PrefKey, 1f);
        volumeSlider.value = saved;
        ApplyVolume(saved);

        volumeSlider.onValueChanged.AddListener(ApplyVolume);
    }

    private void ApplyVolume(float sliderValue)
    {
        // evita Log10(0)
        float lin = Mathf.Clamp(sliderValue, 0.0001f, 1f);
        float db  = Mathf.Log10(lin) * 20f;
        masterMixer.SetFloat("MasterVolume", db);

        PlayerPrefs.SetFloat(PrefKey, sliderValue);
    }
}
