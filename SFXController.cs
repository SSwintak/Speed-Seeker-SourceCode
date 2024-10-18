using MadWise.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SFXController : MonoBehaviour
{

    [System.Serializable]
    public struct SoundEffect
    {
        public string category;
        public List<AudioClip> clips;
    }

    [SerializeField] List<SoundEffect> _SoundEffects = new();

    List<AudioSource> _AudioSources = new();

    void Awake()
    {
        Init();
    }

    private void Init()
    {
        for (int i = 0; i < _SoundEffects.Count; i++)
        {
            GameObject obj = new();
            obj = Instantiate<GameObject>(obj, transform);
            var comp = obj.AddComponent<AudioSource>();
            comp.outputAudioMixerGroup = SettingsManager.Instance.SFXMixer;
            _AudioSources.Add(comp);
        }
    }

    public void PlayRandomSoundEffect(string effectCategoryName)
    {
        if (_SoundEffects.Count == 0 || !_SoundEffects.BurstFind(sfx => sfx.category == effectCategoryName, out SoundEffect effect)) return;
        
        int randomIndex = Random.Range(0, effect.clips.Count);
        AudioClip randomClip = effect.clips[randomIndex];

        StartCoroutine(PlaySoundEffect(randomClip));
    }

    public void PlaySoundEffect(string effectCategoryName, int index)
    {
        if (_SoundEffects.Count == 0 || !_SoundEffects.BurstFind(sfx => sfx.category == effectCategoryName, out SoundEffect effect)) return;

        AudioClip clip = effect.clips[index];
        StartCoroutine(PlaySoundEffect(clip));
    }

    IEnumerator PlaySoundEffect(AudioClip clip)
    {
        if (_AudioSources.BurstFind(source => source.isPlaying == false, out AudioSource _source))
        {
            _source.clip = clip;
            _source.Play();
        }
        yield return null;
    }
}
