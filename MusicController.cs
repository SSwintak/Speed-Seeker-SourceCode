using Sirenix.OdinInspector;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class MusicController : MonoBehaviour
{
    [SerializeField] bool playRandom = true;

    AudioSource audioSource;

    Coroutine musicCoroutine;

    /// <summary>
    /// Max is 20
    /// </summary>
    public static float AudioMixerMax => 20f;

    /// <summary>
    /// Min is -80
    /// </summary>
    public static float AudioMixerMin => -80f;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        Init();
        PlaySong();
    }

    private void OnDestroy()
    {
        if (musicCoroutine != null)
        {
            StopCoroutine(musicCoroutine);
            musicCoroutine = null;
        }
    }
    
    /// <summary>
    /// Will play song(s) repeatedly for specific scene songs, i.e. only plays menu songs in main menu scene
    /// </summary>
    /// <remarks>
    /// Will stop the active coroutine and start it again
    /// </remarks>
    void PlaySong()
    {
        if (musicCoroutine != null) StopCoroutine(musicCoroutine);
        if (playRandom && SceneManager.GetActiveScene().buildIndex == 0) // main menu
        {            
            PlayMenuSong("", playRandom);
            musicCoroutine = StartCoroutine(AutoPlayNext(true));
        }
        else
        {
            PlayLevelSong("", playRandom);
            musicCoroutine = StartCoroutine(AutoPlayNext(false));
        }
    }

    void Init()
    {
        SetVolume(SettingsManager.Instance.MasterMixer);
        SetVolume(SettingsManager.Instance.MusicMixer);
        SetVolume(SettingsManager.Instance.SFXMixer);
    }

    /// <summary>
    /// Converts value of the slider into a Log10 value
    /// </summary>
    float ToLog10(Slider slider)
    {
        float x = Mathf.Log10(slider.value) * 20f;
        return x;
    }

    private void SetVolumeText(Slider slider, TMP_Text text)
    {
        text.text = Mathf.Round(slider.value * 100f).ToString();
    }

    public void SetVolume(AudioMixerGroup audio)
    {
        float vol = 0f;
        if (audio.name == "Master")
        {            
            vol = ToLog10(SettingsManager.Instance.SettingsController.MasterSlider);
            vol = Mathf.Clamp(vol, AudioMixerMin, AudioMixerMax);
            audio.audioMixer.SetFloat(audio.name, vol);
            SetVolumeText(SettingsManager.Instance.SettingsController.MasterSlider, 
                          SettingsManager.Instance.SettingsController.MasterVolumeText);
        }

        else if (audio.name == "Music")
        {
            vol = ToLog10(SettingsManager.Instance.SettingsController.MusicSlider);
            vol = Mathf.Clamp(vol, AudioMixerMin, AudioMixerMax);
            audio.audioMixer.SetFloat(audio.name, vol);
            SetVolumeText(SettingsManager.Instance.SettingsController.MusicSlider,
                          SettingsManager.Instance.SettingsController.MusicVolumeText);
        }

        else
        {
            vol = ToLog10(SettingsManager.Instance.SettingsController.SFXSlider);
            vol = Mathf.Clamp(vol, AudioMixerMin, AudioMixerMax);
            audio.audioMixer.SetFloat(audio.name, vol);
            SetVolumeText(SettingsManager.Instance.SettingsController.SFXSlider,
                          SettingsManager.Instance.SettingsController.SFXVolumeText);
        }
    }

    IEnumerator AutoPlayNext(bool inMainMenu)
    {
        while (true)
        {
            yield return new WaitForSeconds(audioSource.clip.length);
            audioSource.clip = inMainMenu ? MusicPlayer.Instance.GetMenuSong("", playRandom) : MusicPlayer.Instance.GetLevelSong("", playRandom);
            audioSource.Play();
        }
    }

    public void PlayMenuSong(string song, bool _random)
    {
        audioSource.clip = MusicPlayer.Instance.GetMenuSong(song, _random);
        audioSource.Play();
    }

    public void PlayLevelSong(string song, bool _random)
    {
        audioSource.clip = MusicPlayer.Instance.GetLevelSong(song, _random);
        audioSource.Play();
    }


    // editor script buttons
#if UNITY_EDITOR

    [Button("Skip Song")]
    void Button_SkipSong()
    {
        PlaySong();
    }

#endif
}
