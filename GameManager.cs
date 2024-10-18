using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class SavePath
{    
    public static string SaveFilePath => ES3Settings.defaultSettings.FullPath;
}

public class GameManager : MonoBehaviour
{
    static GameManager _Instance;
    public static GameManager Instance => _Instance;
    
    static string CashSaveKey => "Cash";

    object _PlayerCashLock = new object();
    Cash _PlayerCash = new Cash();

    Stopwatch _LevelTimer = new Stopwatch();
    public Stopwatch LevelTimer => _LevelTimer;

    Stopwatch _DifficultyTimer = new Stopwatch();

    Coroutine _DifficultyTimerRoutine;

    Difficulty _GameDifficulty = new Difficulty();
    public Difficulty GameDifficulty => _GameDifficulty;

    public bool AdventureMode = false;
    [ReadOnly, SerializeField] bool _IsGamePaused;
    public bool IsGamePaused => _IsGamePaused;

    [SerializeField] float _TimeBetweenDifficulties = 180f;

    /// <summary>
    /// Is NOT the name of the scene but of the level which can be anything, do NOT make the same as scene name.
    /// </summary>
    public string CurrentLevelName { get; private set; }

    // don't really need a delegate event for this... but there kinda cool
    public delegate void CashUpdated();
    public static event CashUpdated cashChanged;

    public delegate void HandleTimer(bool isRunning);

    /// <summary>
    /// returns true or false depending on if the LevelTimer is running or not when the status changes
    /// </summary>
    public static event HandleTimer onTimerStatusChanged;

    public delegate void DifficultyChange(DifficultyLevel level);
    public static event DifficultyChange onDifficultyChange;

    public delegate void PauseStateChanged(bool isPaused);
    public static event PauseStateChanged onPauseStateChanged;

    /// <summary>
    /// Will create the instance and put in DontDestroyOnLoad
    /// </summary>
    void CreateInstance()
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

    void Awake()
    {
        CreateInstance();
        LoadData();

        SceneManager.sceneLoaded += OnSceneLoad;
    }

    void OnDestroy()
    {
        ES3.Save(CashSaveKey, _PlayerCash.Balance);
        SceneManager.sceneLoaded -= OnSceneLoad;
    }

    void OnSceneLoad(Scene scene, LoadSceneMode mode)
    {
        ResetDifficulty();
        if (scene.buildIndex != 0)// any scene but main menu
        {
            _DifficultyTimerRoutine = StartCoroutine(ChangeDifficultyByTime());
            ObjectPoolManager.ObjectPools.Clear();
        }
        else
        {
            if (_DifficultyTimerRoutine != null) StopCoroutine(_DifficultyTimerRoutine);
        }
    }

    public void ResetDifficulty()
    {
        _GameDifficulty.difficultyLevel = DifficultyLevel.Slow;
    }

    IEnumerator ChangeDifficultyByTime()
    {
        _DifficultyTimer?.Restart();
        while (_GameDifficulty.difficultyLevel != DifficultyLevel.SpeedSeeker)
        {
            // wait for constant length of time then increase difficulty
            yield return null;

            if (_DifficultyTimer.Elapsed.Seconds >= (10))// later will be 3 minutes or 180 seconds
            {
                _GameDifficulty.difficultyLevel++;
                onDifficultyChange?.Invoke(_GameDifficulty.difficultyLevel);
                _DifficultyTimer?.Restart();
            }
        }

        _DifficultyTimer?.Stop();
    }

    public void SetAdventureMode(bool value)
    {
        AdventureMode = value;
    }
    public void SetAdventureMode(Toggle toggle)
    {
        AdventureMode = toggle.isOn;
    }

    public void SetCurrentLevelName(string name)
    {
        CurrentLevelName = name;
    }

    /// <summary>
    /// Starts the timer, does not reset will continue where it left off.
    /// </summary>
    public void StartLevelTimer()
    {
        _LevelTimer.Start();
        onTimerStatusChanged?.Invoke(true);
    }

    /// <summary>
    /// Will reset and start the timer again.
    /// </summary>
    public void RestartLevelTimer()
    {
        StopLevelTimer();
        ResetLevelTimer();
        StartLevelTimer();
    }

    public void StopLevelTimer()
    {
        _LevelTimer.Stop();
        onTimerStatusChanged?.Invoke(false);
    }

    public void ResetLevelTimer()
    {
        _LevelTimer.Reset();
    }    

    public void AddCash(int amount)
    {
        // mutex lock, so multiple shit fucks can't dip their hands in my fucking cookie jar
        lock (_PlayerCashLock)
        {
            _PlayerCash.Balance += amount;            
        }
        if(cashChanged != null) cashChanged(); // so like check if it has at least one subscriber
    }
    public void RemoveCash(int amount)
    {
        lock (_PlayerCashLock)
        {
            _PlayerCash.Balance -= amount;
        }
        if (cashChanged != null) cashChanged();
    }

    public int GetCash() { return _PlayerCash.Balance; }

    public static void DeleteSave(string path)
    {
        ES3.DeleteFile(path);
    }
    private void LoadData()
    {
        if (!ES3.FileExists(SavePath.SaveFilePath))
        {
            UnityEngine.Debug.LogWarning("save file doesn't exist!");
            return;
        }        
       
        _PlayerCash.Balance = ES3.Load<int>(CashSaveKey, 0);
    }

    public void PauseGame()
    {
        AudioListener.pause = true;
        Time.timeScale = 0;
        _IsGamePaused = true;

        onPauseStateChanged?.Invoke(true);
    }

    public void ResumeGame()
    {
        AudioListener.pause = false;
        Time.timeScale = 1;
        _IsGamePaused = false;

        onPauseStateChanged?.Invoke(false);
    }

    public void RestartLevel()
    {
        LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoadScene(string sceneName, bool async = true)
    {
        if (SceneManager.GetSceneByName(sceneName) == null) 
        {
            UnityEngine.Debug.LogError("Scene: " + sceneName + " does not exist!");
            return;
        }
        if (async) SceneManager.LoadSceneAsync(sceneName);
        else SceneManager.LoadScene(sceneName);
    }
}
