
using TMPro;
using UnityEngine;

public class LevelMenuUI : MonoBehaviour
{
    string levelSaveKey => _LevelName;

    [SerializeField] string _LevelName;
    [SerializeField] TMP_Text _BestTimeValueText;
    [SerializeField] TMP_Text _FastestTraveledValueText;
    [SerializeField] TMP_Text _DistanceTraveledValueText;

    void Start()
    {
        if (ES3.FileExists(SavePath.SaveFilePath))
        {
            LoadSave();
        }        
    }
    void LoadSave()
    {
        if (ES3.KeyExists(levelSaveKey))
        {
            LevelSaveData data = ES3.Load<LevelSaveData>(levelSaveKey);
            if (data != null && _LevelName == data.LevelName)
            {
                _BestTimeValueText.text = data.BestTimeAchieved.ToString("hh:mm:ss");
                _FastestTraveledValueText.text = data.BestSpeedAchieved.ToString();
                _DistanceTraveledValueText.text = data.BestDistanceAchieved.ToString();
            }
        }
    }
}
