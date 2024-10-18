
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;



public class MusicPlayer : MonoBehaviour
{
    [System.Serializable]
    public struct Music
    {
        public string author;
        public string songName;
        public AudioClip song;
    }
    public static MusicPlayer Instance { get; private set; }

    [SerializeField] SerializedDictionary<string, Music> _MenuSongs = new SerializedDictionary<string, Music>();
    [SerializeField] SerializedDictionary<string, Music> _LevelSongs = new SerializedDictionary<string, Music>();


    [ShowInInspector, ReadOnly] List<string> _PlayedInSession = new();

    private void CreateInstance()
    {
        if (Instance && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    private void Awake()
    {
        CreateInstance();
        SceneManager.sceneLoaded += OnSceneLoad;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoad;
    }

    void OnSceneLoad(Scene scene, LoadSceneMode mode)
    {
        _PlayedInSession.Clear();
    }

    public AudioClip GetLevelSong(string songName, bool random = false)
    {
        // for 'random' the 'songName' can be null so if the user wants to play a random song ignore the parameter 'songName'
        if (!_LevelSongs.ContainsKey(songName) && !random)
        {
            Debug.LogError($"Error, the song '{songName}' does not exist!");
            return null;
        }

        if (random)
        {
            int randomIndex = Random.Range(0, _LevelSongs.Count);
            string randomKey = _LevelSongs.Keys.ElementAt(randomIndex);

            // if all songs have been shuffled through reset the play session
            if (_PlayedInSession.Count == _LevelSongs.Count) _PlayedInSession.Clear();

            // if the song has already been played find one that hasn't
            while (_PlayedInSession.Contains(randomKey))
            {
                print($"Already played '{randomKey}' finding a new song...");
                randomIndex = Random.Range(0, _LevelSongs.Count);
                randomKey = _LevelSongs.Keys.ElementAt(randomIndex);
            }
            if (!_PlayedInSession.Contains(randomKey)) _PlayedInSession.Add(randomKey);            

            return _LevelSongs[randomKey].song;
        }

        return _LevelSongs[songName].song;
    }
    public AudioClip GetMenuSong(string songName, bool random = false)
    {
        if (!_MenuSongs.ContainsKey(songName) && !random)
        {
            Debug.LogError($"Error, the song '{songName}' does not exist!");
            return null;
        }
            Music song;

        if (SettingsManager.Instance.SettingsController == null)
        {
            SettingsManager.Instance.RetrieveSettingController();
            Assert.IsNotNull(SettingsManager.Instance.SettingsController);
        }

        if (random)
        {
            int randomIndex = Random.Range(0, _MenuSongs.Count);
            string randomKey = _MenuSongs.Keys.ElementAt(randomIndex);

            song = _MenuSongs[randomKey];
            SettingsManager.Instance.SettingsController.MusicCreditsText.text =
            song.songName + " by " + song.author;

            return song.song;
        }

        song = _MenuSongs[songName];

        SettingsManager.Instance.SettingsController.MusicCreditsText.text =
        song.songName + " by " + song.author;

        return song.song;
    }
}
