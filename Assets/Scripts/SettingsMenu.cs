using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] GameObject settingsPanel;
    [SerializeField] Slider      musicSlider;

    const string PrefKey = "MasterVolume";

    void Start()
    {
        float saved = PlayerPrefs.GetFloat(PrefKey, 1f);
        musicSlider.value = saved;
        ApplyVolume(saved);

        musicSlider.onValueChanged.AddListener(ApplyVolume);
    }

    void ApplyVolume(float value)
    {
        // volume global: 0-1
        AudioListener.volume = Mathf.Clamp01(value);
        PlayerPrefs.SetFloat(PrefKey, value);
    }

    public void OpenSettings()  => settingsPanel.SetActive(true);
    public void CloseSettings() => settingsPanel.SetActive(false);
}