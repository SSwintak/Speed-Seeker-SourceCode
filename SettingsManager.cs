
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using UnityEngine.Audio;

public class SettingsManager : MonoBehaviour
{
    static SettingsManager _Instance;
    public static SettingsManager Instance => _Instance;
    private void CreateInstance()
    {
        if (_Instance && _Instance != this)
        {
            Destroy(this);
        }
        else
        {
            _Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    string audioSettingsSaveKey => "AudioSettings";
    string graphicsSettingsSaveKey => "GraphicSettings";

    SettingsController settingsController;

    [SerializeField] AudioMixerGroup _MasterMixer;
    [SerializeField] AudioMixerGroup _MusicMixer;
    [SerializeField] AudioMixerGroup _SFXMixer;

    public AudioMixerGroup MasterMixer => _MasterMixer;
    public AudioMixerGroup MusicMixer => _MusicMixer;
    public AudioMixerGroup SFXMixer => _SFXMixer;

    public SettingsController SettingsController => settingsController;

    private void Awake()
    {
        CreateInstance();
        Init();
    }

    void Init()
    {
        settingsController = FindObjectOfType<SettingsController>(true);
        Assert.IsNotNull(settingsController, "Error, could not find an object with the 'SettingsController' component.");        
    }    

    /// <summary>
    /// Will load settings from save file and retrieve the settings controller it is null.
    /// </summary>
    public void InitSettings()
    {
        if (!settingsController)
        {
            Init(); // try to find the controller again
            if (!settingsController) return; // if you can't find still GTFO
        }

        if (ES3.FileExists(SavePath.SaveFilePath))
        {
            if (ES3.KeyExists(audioSettingsSaveKey) && ES3.KeyExists(graphicsSettingsSaveKey))
            {
                AudioSettingsSaveData audioData = ES3.Load<AudioSettingsSaveData>(audioSettingsSaveKey);

                GraphicSettingsSaveData graphicData = ES3.Load<GraphicSettingsSaveData>(graphicsSettingsSaveKey);

                settingsController.AudioSettings.LoadAudioSettings(audioData);
                settingsController.GraphicSettings.LoadGraphicSettings(graphicData);

                return;
            }
        }        
    }

    public void RetrieveSettingController()
    {
        if (settingsController) return;
        settingsController = FindObjectOfType<SettingsController>(true);
    }

    public void SaveSettings()
    {

        AudioSettingsSaveData audioSaveData = new AudioSettingsSaveData(
        settingsController.AudioSettings.masterSlider.value,
        settingsController.AudioSettings.musicSlider.value,
        settingsController.AudioSettings.SFXSlider.value,
        settingsController.AudioSettings.playUnfocusedToggle.isOn
        );

        GraphicSettingsSaveData graphicSettingsSaveData = new GraphicSettingsSaveData(
        settingsController.GraphicSettings.screenModeDropDown.value,
        settingsController.GraphicSettings.resolutionDropDown.value,
        settingsController.GraphicSettings.graphicsQualityDropDown.value,
        settingsController.GraphicSettings.FPSLimitDropDown.value,
        settingsController.GraphicSettings.VSYNC_Toggle.isOn,
        settingsController.GraphicSettings.musicCreditsToggle.isOn
        );

        ES3.Save(audioSettingsSaveKey, audioSaveData);

        ES3.Save(graphicsSettingsSaveKey, graphicSettingsSaveData);
    }

}
