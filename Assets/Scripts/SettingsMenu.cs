    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.Audio;


    public class SettingsMenu : MonoBehaviour
    {
        public GameObject settingsPanel;
        public Slider musicSlider;
        public static float MusicVolume = 1f;
        public AudioMixer mixer;



        void Start()
        {
            musicSlider.value = PlayerPrefs.GetFloat("MasterVolume", 1f);

            // Aplicar volumes salvos ao mixer
            mixer.SetFloat("MasterVolume", Mathf.Log10(musicSlider.value) * 20);

            musicSlider.onValueChanged.AddListener(SetMusicVolume);
        }


        public void SetMusicVolume(float value)
        {
            mixer.SetFloat("MasterVolume", Mathf.Log10(value) * 20);
            MusicVolume = value;
            PlayerPrefs.SetFloat("MasterVolume", value);
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
