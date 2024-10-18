using Sirenix.OdinInspector;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SpeedSeeker.Settings
{
    [Serializable]
    public class AudioSettings
    {
        [ES3NonSerializable] public Slider masterSlider;
        [ES3NonSerializable] public Slider musicSlider;
        [ES3NonSerializable] public Slider SFXSlider;
        [ES3NonSerializable] public Toggle playUnfocusedToggle;

        public AudioSettings(AudioSettings settings)
        {
            masterSlider= settings.masterSlider;
            musicSlider= settings.musicSlider;
            SFXSlider= settings.SFXSlider;
            playUnfocusedToggle= settings.playUnfocusedToggle;
        }

        public void LoadAudioSettings(AudioSettingsSaveData data)
        {
            masterSlider.value = data.masterVolume;
            musicSlider.value = data.musicVolume;
            SFXSlider.value = data.sfxVolume;
            playUnfocusedToggle.isOn = data.playUnfocused;
        }
    }

    [Serializable]
    public class GraphicSettings
    {
        [ES3NonSerializable] public TMP_Dropdown screenModeDropDown;
        [ES3NonSerializable] public TMP_Dropdown resolutionDropDown;
        [ES3NonSerializable] public TMP_Dropdown graphicsQualityDropDown;
        [ES3NonSerializable]  public TMP_Dropdown FPSLimitDropDown;

        [ES3NonSerializable] public Toggle VSYNC_Toggle;
        [ES3NonSerializable] public Toggle musicCreditsToggle;

        public GraphicSettings(GraphicSettings settings)
        {
            screenModeDropDown= settings.screenModeDropDown;
            resolutionDropDown= settings.resolutionDropDown;
            graphicsQualityDropDown= settings.graphicsQualityDropDown;
            FPSLimitDropDown= settings.FPSLimitDropDown;
            VSYNC_Toggle= settings.VSYNC_Toggle;
            musicCreditsToggle= settings.musicCreditsToggle;
        }

        public void LoadGraphicSettings(GraphicSettingsSaveData data)
        {
            screenModeDropDown.value = data.screenModeOption;
            resolutionDropDown.value = data.resolutionOption;
            graphicsQualityDropDown.value = data.graphicsQualityOption;
            FPSLimitDropDown.value = data.fpsLimitOption;
            VSYNC_Toggle.isOn = data.vSyncOn;
            musicCreditsToggle.isOn = data.musicCreditsOn;
        }
    }

    [Serializable]
    public class ControlSettings
    {

    }
}

public class SettingsController : MonoBehaviour
{

    [BoxGroup("Audio Volume Text")]
    [SerializeField] TMP_Text _MasterVolumeText, _MusicVolumeText, _SFXVolumeText;

    [BoxGroup("Audio Settings")]
    [SerializeField] SpeedSeeker.Settings.AudioSettings _AudioSettings;

    [BoxGroup("Graphic Settings")]
    [SerializeField] SpeedSeeker.Settings.GraphicSettings _GraphicSettings;

    [SerializeField] TMP_Text _MusicCreditsText;

    public TMP_Text MasterVolumeText => _MasterVolumeText;
    public TMP_Text MusicVolumeText => _MusicVolumeText;
    public TMP_Text SFXVolumeText => _SFXVolumeText;

    public Slider MasterSlider => _AudioSettings.masterSlider;
    public Slider MusicSlider => _AudioSettings.musicSlider;
    public Slider SFXSlider => _AudioSettings.SFXSlider;
    public SpeedSeeker.Settings.AudioSettings AudioSettings => _AudioSettings;
    public SpeedSeeker.Settings.GraphicSettings GraphicSettings => _GraphicSettings;
    public TMP_Text MusicCreditsText => _MusicCreditsText;

    private void Start()
    {
        SettingsManager.Instance.InitSettings();
        Init();
    }

    private void Init()
    {
        SetMusicCredits(_GraphicSettings.musicCreditsToggle);
        SetVSync(_GraphicSettings.VSYNC_Toggle);
        SetResolution(_GraphicSettings.resolutionDropDown);
        SetFrameRateCap(_GraphicSettings.FPSLimitDropDown);
        SetRunInBackground(_AudioSettings.playUnfocusedToggle);
        SetGraphicsQuality(_GraphicSettings.graphicsQualityDropDown);
        SetScreenMode(_GraphicSettings.screenModeDropDown);
    }
    public void SaveSettings()
    {
        SettingsManager.Instance.SaveSettings();
    }

    public void SetMusicCredits(Toggle toggle)
    {
        _MusicCreditsText.gameObject.SetActive(toggle.isOn);
    }

    public void SetVSync(Toggle toggle)
    {
        QualitySettings.vSyncCount = toggle.isOn ? 1 : 0; // 1 is on 0 turn it off
        VSyncSetFrameRateCap();
    }

    public void SetResolution(TMP_Dropdown dropdown)
    {
        // get the current selected options text
        int option = dropdown.value;
        string resolution = dropdown.options[option].text;

        string[] parts = resolution.Split('-');

        int width = 1920;
        int height = 1080;
        if (parts.Length >= 2 ) 
        {
            // converting to int can be expensive my guy
            width = int.Parse(parts[0]);
            height = int.Parse(parts[1]);
        }
        
        Screen.SetResolution(width, height, true);
    }

    private void VSyncSetFrameRateCap()
    {
        // if every V blank; fps = 60
        // if every second V blank; fps = 30
        // if every thrice V blank; fps = 20
        if (_GraphicSettings.VSYNC_Toggle.isOn)
        {
            Application.targetFrameRate = 60;
            _GraphicSettings.FPSLimitDropDown.value = 1; // location of "60 fps" in the dropdown
            _GraphicSettings.FPSLimitDropDown.interactable = false;
        }
        else
        {
            _GraphicSettings.FPSLimitDropDown.interactable = true;
        }
    }

    public void SetFrameRateCap(TMP_Dropdown dropdown)
    {
        int option = dropdown.value;
        string frameRate = dropdown.options[option].text;

        int targetFrameRate = 60;

        string[] parts = frameRate.Split("fps");

        if (parts.Length > 0 && int.TryParse(parts[0], out targetFrameRate))
        {
            Application.targetFrameRate = targetFrameRate;
        }
        else
        {
            Application.targetFrameRate = -1;
        }
    }

    public void SetRunInBackground(Toggle toggle)
    {
        Application.runInBackground = toggle.isOn;
    }

    public void SetGraphicsQuality(TMP_Dropdown dropdown)
    {
        QualitySettings.SetQualityLevel(dropdown.value);
    }

    public void SetScreenMode(TMP_Dropdown dropdown)
    {
        switch (dropdown.options[dropdown.value].text)
        {
            case "FullScreen":
                Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
                break;

            case "Windowed":
                Screen.fullScreenMode = FullScreenMode.Windowed;
                break;

            default: break;
        }
    }
}
