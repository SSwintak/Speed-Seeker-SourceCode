using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class AchievementController : MonoBehaviour
{
    string currentLevelSaveKey;
    string currentLevelName;

    [SerializeField] TMP_Text _SpeedValueText;
    [SerializeField] TMP_Text _DistanceValueText;
    [SerializeField] TMP_Text _TimeValueText;
    [SerializeField] GameObject _TimeObject;
    [SerializeField] GameObject _FuelObject;
    [SerializeField] TMP_Text _CashValueText;
    [SerializeField] GameObject _PlayerObject;
    [SerializeField, ReadOnly] bool _AdventureMode = false;

    TimeInfo _CurrentTime;
    Player _Player;
    int _CurrentDistance;

    int _BestSpeed;
    TimeInfo _BestTime;
    int _BestDistance;

    float _TimeBetweenDistanceUpdate = 1; // 1 second

    void Awake()
    {
        Init();

        if (_AdventureMode) InitTimer();
        else
        {
            _TimeValueText.transform.parent.gameObject.SetActive(false);
        }
    }
    void OnDestroy()
    {
        if (_AdventureMode) GameManager.onTimerStatusChanged -= OnTimerStatusChanged;
    }
    void Update()
    {

        if (_AdventureMode) UpdateTimer();

        _SpeedValueText.text = _Player.CurrentSpeed.ToString();
        if (_BestSpeed < _Player.BestSpeed) _BestSpeed = _Player.BestSpeed;

        _CashValueText.text = GameManager.Instance.GetCash().ToString();

        UpdateDistanceTravelled();
    }

    private void UpdateTimer()
    {
        _CurrentTime = new TimeInfo(GameManager.Instance.LevelTimer.Elapsed);
        _TimeValueText.text = _CurrentTime.ToString("hh:mm:ss");
        if (_BestTime < _CurrentTime) _BestTime = _CurrentTime;
    }

    void Init()
    {
        currentLevelName = GameManager.Instance.CurrentLevelName;
        currentLevelSaveKey = currentLevelName;
        _Player = _PlayerObject.GetComponent<Player>();
        _AdventureMode = GameManager.Instance.AdventureMode;
    }

    void InitTimer()
    {
        _TimeObject.SetActive(true);
        _FuelObject.SetActive(false);
        GameManager.onTimerStatusChanged += OnTimerStatusChanged;
        GameManager.Instance.RestartLevelTimer();
    }

    public void StopTimer()
    {
        GameManager.Instance.StopLevelTimer();
    }

    void OnTimerStatusChanged(bool isRunning)
    {
        if (!isRunning)
        {
            SaveLevelAchievements();
        }
    }

    void UpdateDistanceTravelled()
    {
        if (_TimeBetweenDistanceUpdate >= 0.5f && _Player.CurrentSpeed > 0f)
        {
            _CurrentDistance += (int)(_Player.CurrentSpeed * 0.5f);
            if (_BestDistance < _CurrentDistance) _BestDistance = _CurrentDistance;
            _DistanceValueText.text = _CurrentDistance.ToString();
            _TimeBetweenDistanceUpdate = 0f;
        }

        _TimeBetweenDistanceUpdate += Time.deltaTime;
    }

    /// <summary>
    /// If enabled there will be a timer for best timed run.
    /// </summary>
    public void SetModeAdventure(bool value)
    {
        _AdventureMode = value;
    }

    public void SaveLevelAchievements()
    {
        string saveKey = GameManager.Instance.CurrentLevelName;
        LevelSaveData data = new();

        if (ES3.KeyExists(saveKey)) // check to see if we've saved any achievements
        {
            LevelSaveData savedData = ES3.Load<LevelSaveData>(saveKey);
            if (savedData != null && saveKey == savedData.LevelName) // if so check if the player has beaten any milestones
            {
                data.LevelName = savedData.LevelName;
                data.BestSpeedAchieved = _BestSpeed > savedData.BestSpeedAchieved ? _BestSpeed : savedData.BestSpeedAchieved;
                data.BestTimeAchieved = _BestTime > savedData.BestTimeAchieved ? _BestTime : savedData.BestTimeAchieved;
                data.BestDistanceAchieved = _BestDistance > savedData.BestDistanceAchieved ? _BestDistance : savedData.BestDistanceAchieved;
            }
        }
        else data = new LevelSaveData(currentLevelName, _BestSpeed, _BestTime, _BestDistance); // if we haven't saved any achievements yet then save what we have

        ES3.Save(currentLevelSaveKey, data);
    }
}
