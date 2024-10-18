using Febucci.UI;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class ShipMenuData : MonoBehaviour
{   

    bool _IsLocked = true;

    [Header("Ship Purchase Options")]

    [SerializeField] int _PurchaseAmount;
    [SerializeField] int _BasePurchaseLevelAmount;
    int _CurrentPurchaseLevelAmount;

    [SerializeField] float _PurchaseLevelMultiplier = 1.1f;

    [Header("Ship Interactive UI")]

    [SerializeField] GameObject _Locked;
    [SerializeField, EnableIf("_Locked")] TypewriterByCharacter _LockedTypewriter;
    [SerializeField] GameObject _Toggle;
    [SerializeField] GameObject _Selected;
    [SerializeField] GameObject _Unlock;
    [SerializeField] GameObject _LevelUp;

    [Header("Ship Level UI")]

    [SerializeField] TMP_Text _PriceValueText;

    [Tooltip("The object that stores all the images to represent the level")]
    [SerializeField] GameObject _ShipLevelValueObject;

    [SerializeField] GameObject _IncreaseLevelButton;
    [SerializeField] TMP_Text _LevelValueText;
    TMP_Text _LevelUpPriceText;

    [Header("Ship Stats UI")]

    [Tooltip("The scriptable object relating to the ship itself, "+
             "i.e. if this is the selectable for the ship Flea, then reference the Flea scriptable object")]
    [SerializeField] ShipObject _ShipObject;

    [SerializeField] List<TMP_Text> _ShipStatsText = new List<TMP_Text>();

    [SerializeField] GameObject _ShipStats;

    [SerializeField] GameObject _ShipShield;


    public ShipStats shipStats { get; private set; }
    
    string saveLevelPurchaseKey => name + " CurrentLevelPurchaseAmount";
    string saveIsSelectedKey => name + " IsSelected";

    int balance => GameManager.Instance.GetCash();

    bool _isSelected = false;

    public delegate void HasSelectedShip(bool value);
    public static event HasSelectedShip OnHasSelectedShip;

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        if (!ES3.FileExists(SavePath.SaveFilePath) || !ShipGarage.Instance.HasShip(_ShipObject.Name))
        {
            shipStats = new ShipStats(_ShipObject);
        }
        else
        {            
            UpdateShipStats();
            ShowShipFromGarage();
            LoadShipLevelsUI();
        }
        LoadPriceObjects();
        SetLevelText(shipStats.level);
        UpdateShipMenuStats();
    }

    private void LoadPriceObjects()
    {
        _CurrentPurchaseLevelAmount = ES3.Load<int>(saveLevelPurchaseKey, _BasePurchaseLevelAmount);
        if (_CurrentPurchaseLevelAmount <= 0)
        {
            _CurrentPurchaseLevelAmount = _BasePurchaseLevelAmount;
        }
        _PriceValueText.text = _PurchaseAmount.ToString();
        _LevelUpPriceText = _IncreaseLevelButton.GetComponentInChildren<TMP_Text>(true);
        _LevelUpPriceText.text = _CurrentPurchaseLevelAmount.ToString();
    }

    private void ShowShipFromGarage()
    {
        Assert.IsNotNull(ShipGarage.Instance, "Error, ShipGarage Instance is null!");
        if (!ShipGarage.Instance.HasShip(_ShipObject.Name)) return;

        _IsLocked = false;

        Destroy(_Locked);
        Destroy(_Unlock);

        _Toggle.SetActive(true);
        _ShipStats.SetActive(true);
        _LevelUp.SetActive(true);
        
        _isSelected = ES3.Load<bool>(saveIsSelectedKey, false);
        if (_isSelected) SelectShip();
    }

    private void UpdateShipMenuStats()
    {
        Assert.IsNotNull(_ShipStatsText, "Error, ShipStatsText is null!");
        Assert.IsNotNull(shipStats, "Error, shipStats is null!");
        foreach (var stat in _ShipStatsText)
        {            
            switch (stat.name)
            {
                case "Speed":                    
                    stat.text = "Speed: " + shipStats.speed.ToString();
                    break;

                case "Acceleration":
                    stat.text = "Accel: " + shipStats.acceleration.ToString();
                    break;

                case "HitPoints":
                    stat.text = "HitPoints: " + shipStats.hitPoints.ToString();
                    break;

                case "Fuel":
                    stat.text = "Fuel: " + shipStats.fuel.ToString();
                    break;

                default: break;
            }

        }
    }

    public void SelectShip()
    {
        ShipGarage.Instance.SelectedShip = _ShipObject.Name;
        _Selected.SetActive(!_Selected.activeSelf);
        _isSelected = _Selected.activeSelf;

        // need this to prevent the ToggleGroup from invoking the IsOn to true
        // for the first toggle in the group when loading and have selected another ship
        _Toggle.GetComponent<Toggle>().isOn = _Selected.activeSelf;

        OnHasSelectedShip(_isSelected);

        ES3.Save(saveIsSelectedKey, _isSelected);
    }

    public void PurchaseShip()
    {
        if (_IsLocked && balance < _PurchaseAmount) return;
        
        Assert.IsNotNull(_LockedTypewriter, "Error, the 'Locked' object has no children with a TypewriterByCharacter component!");

        _LockedTypewriter.onTextDisappeared.AddListener(UnlockShip);

        _LockedTypewriter.waitForNormalChars = 0.24f;

        _LockedTypewriter.StartDisappearingText();

    }

    public void PurchaseLevelUp()
    {
        IncreaseLevel(balance);
    }

    private void UnlockShip()
    {        
        GameManager.Instance.RemoveCash(_PurchaseAmount);

        _IsLocked = false;

        Destroy(_Locked);
        Destroy(_Unlock);

        _Toggle.SetActive(true);
        _ShipStats.SetActive(true);
        _LevelUp.SetActive(true);
        _isSelected = true;

        ShipGarage.Instance.AddShip(shipStats, _ShipObject.Name);

        _LockedTypewriter.onTextDisappeared.RemoveListener(UnlockShip);
    }

    private void IncreaseLevel(int _balance)
    {
        if (_balance < _CurrentPurchaseLevelAmount ||  !_ShipLevelValueObject) return;

        // get the images that represent the level value of the ship, offset by -1 + the current level to get the correct index
        // if the current level is <= 0 return the 0th index else return the index from the offset + current level i.e > 0
        int childIndex = shipStats.level <= 0 ? 0 : 0 + shipStats.level;

        GameObject levelValue = _ShipLevelValueObject.transform.GetChild(childIndex).gameObject;
        levelValue.SetActive(true);

        // get the level by adding 1 to the index of the child
        int currentLevel = childIndex+1;

        // disable the button if the max level is reached
        if (currentLevel == _ShipObject.MaxLevel) _IncreaseLevelButton.SetActive(false);

        CalculateShipStats(currentLevel);        

        GameManager.Instance.RemoveCash(_CurrentPurchaseLevelAmount);

        // increase the cost for the next level up and save it
        _CurrentPurchaseLevelAmount += Mathf.RoundToInt(_CurrentPurchaseLevelAmount * _PurchaseLevelMultiplier);

        _LevelUpPriceText.text = _CurrentPurchaseLevelAmount.ToString();
        SetLevelText(currentLevel);

        ES3.Save(saveLevelPurchaseKey, _CurrentPurchaseLevelAmount);
    }

    private void CalculateShipStats(int level)
    {
        ShipObject shipObject = _ShipObject;
        if (level > shipObject.MaxLevel) return;
        
        // diff = the difference between the base and max speed
        // mlvl = the max level
        // r = rate of change = diff / mlvl
        // speed = min(baseSpeed + level * r, mlvl)
        float rateOfSpeedChange = (_ShipObject.MaxSpeed - _ShipObject.BaseSpeed) / _ShipObject.MaxLevel;
        float speedIncrease = Mathf.Min(_ShipObject.BaseSpeed + level * rateOfSpeedChange, _ShipObject.MaxSpeed);

        float rateOfAccelChange = (_ShipObject.MaxAcceleration- _ShipObject.BaseAcceleration) / _ShipObject.MaxLevel;
        float accelIncrease = Mathf.Min(_ShipObject.BaseAcceleration + level * rateOfAccelChange, _ShipObject.MaxAcceleration);

        float rateOfFuelChange = (_ShipObject.MaxFuel - _ShipObject.BaseFuel) / _ShipObject.MaxLevel;
        float fuelIncrease = Mathf.Min(_ShipObject.BaseFuel + level * rateOfFuelChange, _ShipObject.MaxFuel);

        // don't let the value above or below the min/max
        speedIncrease = Mathf.Clamp(speedIncrease, _ShipObject.BaseSpeed, _ShipObject.MaxSpeed);
        accelIncrease = Mathf.Clamp(accelIncrease, _ShipObject.BaseAcceleration, _ShipObject.MaxAcceleration);
        fuelIncrease = Mathf.Clamp(fuelIncrease, _ShipObject.BaseFuel, _ShipObject.MaxFuel);

        shipStats = new ShipStats(level, speedIncrease, accelIncrease, shipStats.hitPoints, fuelIncrease);
        ShipGarage.Instance.UpdateShipStats(shipStats, _ShipObject.Name);
        UpdateShipMenuStats();
    }

    private void UpdateShipStats()
    {
        if (ShipGarage.Instance.HasShip(_ShipObject.Name))
        {
            shipStats = new ShipStats(ShipGarage.Instance.GetShipStats(_ShipObject.Name));
        }
    }

    private void LoadShipLevelsUI()
    {
        if (_IsLocked) return;

        if (IsMaxLevel())
        {
            _IncreaseLevelButton.SetActive(false);
        }

        Transform levelsParent = _ShipLevelValueObject.transform;
        for (int i = 1; i < shipStats.level; i++)
        {
            levelsParent.GetChild(i).gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// A helper function to the text of the _levelValueText i.e (1/10)
    /// </summary>
    private void SetLevelText(int level)
    {
       _LevelValueText.text = level.ToString() + "/" + _ShipObject.MaxLevel.ToString();
    }

    public bool IsMaxLevel()
    {
        Assert.IsNotNull(shipStats, "shipStats is null!");
        Assert.IsNotNull(_ShipObject, "_ShipObject is null!");
        if (shipStats.level == _ShipObject.MaxLevel) return true;
        return false;
    }

#if UNITY_EDITOR

    //private void OnValidate()
    //{
    //    _PriceValueText.text = _PurchaseAmount.ToString();
    //    RectTransform trans = _PriceValueText.rectTransform;
    //    trans.localPosition = new Vector3(trans.sizeDelta.x / 2f, trans.localPosition.y, 0);
    //}

#endif

}
