using Sirenix.OdinInspector;
using System;
using UnityEngine;

[Serializable]
public class ShipStats
{
    // if you want more control of getters/setters and serialization you need this
    [SerializeField] private int _level;
    [SerializeField] private int _hitPoints;
    [SerializeField] private float _speed;
    [SerializeField] private float _acceleration;
    [SerializeField] private float _fuel;

    public int level
    {
        get { return _level; }
        private set { _level = value; }
    }
    public int hitPoints
    {
        get { return _hitPoints; }
        private set { _hitPoints = value; }
    }
    public float speed
    {
        get { return _speed; }
        private set { _speed = value; }
    }
    public float acceleration
    {
        get { return _acceleration; }
        private set { _acceleration = value; }
    }

    public float fuel
    {
        get { return _fuel; }
        private set { _fuel = value; }
    }

    public void AddHealth(int amount)
    {
        _hitPoints += amount;
        _hitPoints = Mathf.Clamp(_hitPoints, 0, 50);
    }

    public void SubtractHealth(int amount)
    {
        _hitPoints -= amount;
        _hitPoints = Mathf.Clamp(_hitPoints, 0, 50);
    }

    // constructors
    #region 
    public ShipStats(){}

    public ShipStats(ShipStats _stats)
    {
        level = _stats.level;
        speed = _stats.speed;
        acceleration = _stats.acceleration;
        hitPoints = _stats.hitPoints;
        fuel = _stats.fuel;
    }

    public ShipStats(Ship _Ship)
    {
        level = _Ship.stats.level;
        speed = _Ship.stats.speed;
        acceleration = _Ship.stats.acceleration;
        hitPoints = _Ship.stats.hitPoints;
        fuel = _Ship.stats.fuel;
    }

    public ShipStats(int _level, float _speed, float _acceleration, int _hitPoints, float _fuel)
    {
        level = _level;
        speed = _speed;
        acceleration = _acceleration;
        hitPoints = _hitPoints;
        fuel = _fuel;
    }

    public ShipStats(ShipObject _ShipObject, int _level, float _speed, float _acceleration, float _fuel)
    {
        level = _level;
        speed = _speed;
        acceleration = _acceleration;
        fuel = _fuel;
        hitPoints = _ShipObject.HitPoints;
    }

    public ShipStats(ShipObject _ShipObject)
    {        
        level = 1;
        speed = _ShipObject.BaseSpeed;
        acceleration = _ShipObject.BaseAcceleration;
        hitPoints = _ShipObject.HitPoints;
        fuel = _ShipObject.BaseFuel;
    }
    #endregion
}

public class Ship : MonoBehaviour
{
    ShipObject _shipObject;
    public ShipObject shipObject => _shipObject;
    public SpriteRenderer ThrusterRenderer;
    public Vector2 ThrusterOffset;

    [ReadOnly]
    public ShipStats stats;
    SpriteRenderer _ShipRenderer;
    [HideInInspector] public GameObject Shield;

    private void Awake()
    {
        _ShipRenderer = GetComponent<SpriteRenderer>();
        GetSelectedShip();

        Shield = Instantiate(shipObject.ShipShield, transform.parent);
        Shield.transform.localPosition = Vector3.zero;
        Shield.SetActive(false);

        ThrusterOffset = shipObject.ThrusterOffset;
        ThrusterRenderer.sprite = shipObject.ThrusterSprite;
        ThrusterRenderer.transform.localPosition = new Vector3(ThrusterOffset.x, ThrusterOffset.y, 0f);
        DisEngageThruster();
    }

    public void TakeDamage(int damage)
    {
        stats.SubtractHealth(damage);
    }

    public void AddHealth(int amount)
    {
        stats.AddHealth(amount);
    }

    /// <summary>
    /// Will set the ships stats from the selected ship in the garage. See ShipGarage.Instance.SelectedShip
    /// </summary>
    public void GetSelectedShip()
    {
        var garage = ShipGarage.Instance;
        stats = garage.GetShipStats(garage.SelectedShip);
        _shipObject = garage.AllShips[garage.SelectedShip];
        _ShipRenderer.sprite = _shipObject.ShipSprite;
    }

    public void EngageThruster()
    {
        ThrusterRenderer.gameObject.SetActive(true);
    }

    public void DisEngageThruster()
    {
        ThrusterRenderer.gameObject.SetActive(false);
    }

    public bool IsThrusterEngaged()
    {
        return ThrusterRenderer.gameObject.activeSelf;
    }
}
