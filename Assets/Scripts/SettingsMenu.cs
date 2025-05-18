    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.Audio;


    public class SettingsMenu : MonoBehaviour
    {
        public GameObject settingsPanel;
        public Slider musicSlider;
        public Slider sfxSlider;
        public Slider brightnessSlider;
        public CanvasGroup screenCanvasGroup;
        public static float SfxVolume = 1f;
        public static float MusicVolume = 1f;
        public AudioMixer mixer;



        void Start()
        {
            screenCanvasGroup = FindObjectOfType<CanvasGroup>();

            musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1f);
            sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1f);
            brightnessSlider.value = PlayerPrefs.GetFloat("Brightness", 1f);

            // Aplicar volumes salvos ao mixer
            mixer.SetFloat("MusicVolume", Mathf.Log10(musicSlider.value) * 20);
            mixer.SetFloat("SfxVolume", 0f);

            musicSlider.onValueChanged.AddListener(SetMusicVolume);
            sfxSlider.onValueChanged.AddListener(SetSFXVolume);
            brightnessSlider.value = PlayerPrefs.GetFloat("Brightness", 1f);
            screenCanvasGroup.alpha = brightnessSlider.value;
        }


        public void SetMusicVolume(float value)
        {
            mixer.SetFloat("MusicVolume", Mathf.Log10(value) * 20);
            MusicVolume = value;
            PlayerPrefs.SetFloat("MusicVolume", value);
        }


        public void SetSFXVolume(float value)
        {
            mixer.SetFloat("SfxVolume", Mathf.Log10(value) * 20);
            SfxVolume = value;
            PlayerPrefs.SetFloat("SFXVolume", value);
        }


        public void SetBrightness(float value)
        {
            if (screenCanvasGroup != null)
                screenCanvasGroup.alpha = Mathf.Clamp01(value); // valor de 0 (escuro) a 1 (normal)

            PlayerPrefs.SetFloat("Brightness", value);
        }

        public void OpenSettings()
        {
            settingsPanel.SetActive(true);
        }

        public void CloseSettings()
        {
            settingsPanel.SetActive(false);
        }
    }
