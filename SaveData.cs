using System;

[Serializable]
public class SaveData { }


[Serializable]
public class AudioSettingsSaveData : SaveData
{
    public float masterVolume;
    public float musicVolume;
    public float sfxVolume;
    public bool playUnfocused;

    public AudioSettingsSaveData(float _masterVolume, float _musicVolume, float _sfxVolume, bool _playUnfocused)
    {
        masterVolume = _masterVolume;
        musicVolume = _musicVolume;
        sfxVolume = _sfxVolume;
        playUnfocused = _playUnfocused;
    }

    public AudioSettingsSaveData(AudioSettingsSaveData data)
    {
        masterVolume= data.masterVolume;
        musicVolume= data.musicVolume;
        sfxVolume= data.sfxVolume;
        playUnfocused = data.playUnfocused;
    }
}

[Serializable]
public class GraphicSettingsSaveData : SaveData
{
    public int screenModeOption;
    public int resolutionOption;
    public int graphicsQualityOption;
    public int fpsLimitOption;
    public bool vSyncOn;
    public bool musicCreditsOn;

    public GraphicSettingsSaveData(int screenModeOption, int resolutionOption, int graphicsQualityOption, 
                                   int fpsLimitOption, bool vSyncOn, bool musicCreditsOn)
    {
        this.screenModeOption = screenModeOption;
        this.resolutionOption = resolutionOption;
        this.graphicsQualityOption = graphicsQualityOption;
        this.fpsLimitOption = fpsLimitOption;
        this.vSyncOn = vSyncOn;
        this.musicCreditsOn = musicCreditsOn;
    }

    public GraphicSettingsSaveData(GraphicSettingsSaveData data)
    {
        screenModeOption= data.screenModeOption;
        resolutionOption= data.resolutionOption;
        graphicsQualityOption= data.graphicsQualityOption;
        fpsLimitOption= data.fpsLimitOption;
        vSyncOn= data.vSyncOn;
        musicCreditsOn= data.musicCreditsOn;
    }
}

[Serializable]
public class LevelSaveData : SaveData
{
    public string LevelName;
    public int BestSpeedAchieved;
    public TimeInfo BestTimeAchieved;
    public int BestDistanceAchieved;

    public LevelSaveData(string _levelName, int _bestSpeedAchieved, TimeInfo _bestTimeAchieved, int _bestDistanceAchieved)
    {
        LevelName = _levelName;
        BestSpeedAchieved = _bestSpeedAchieved;
        BestTimeAchieved = _bestTimeAchieved;
        BestDistanceAchieved= _bestDistanceAchieved;
    }

    public LevelSaveData(LevelSaveData data)
    {
        LevelName = data.LevelName;
        BestSpeedAchieved = data.BestSpeedAchieved;
        BestTimeAchieved = data.BestTimeAchieved;
        BestDistanceAchieved = data.BestDistanceAchieved;
    }

    public LevelSaveData() { }
}



