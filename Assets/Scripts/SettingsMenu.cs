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

            // Aplicar volumes salvos ao mixer
            mixer.SetFloat("MusicVolume", Mathf.Log10(musicSlider.value) * 20);
            mixer.SetFloat("SfxVolume", 0f);

            musicSlider.onValueChanged.AddListener(SetMusicVolume);
            screenCanvasGroup.alpha = brightnessSlider.value;
        }


        public void SetMusicVolume(float value)
        {
            mixer.SetFloat("MusicVolume", Mathf.Log10(value) * 20);
            MusicVolume = value;
            PlayerPrefs.SetFloat("MusicVolume", value);
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
