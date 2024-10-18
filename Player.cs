using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.Assertions;

public class Player : MonoBehaviour
{

    InputAction _AccelerateAction;


    [SerializeField] Camera _PlayerCamera;
    [SerializeField] Ship ship;
    [SerializeField] PlayerCollision _PlayerCollision;

    // used for the UI
    int _CurrentSpeed;

    public int CurrentSpeed => _CurrentSpeed;
    public int BestSpeed { get; private set; }

    float _SpeedBoostMultiplier = 0;
    float _SpeedBoostDuration;
    float _CurrentSpeedBoostTime;

    [SerializeField] Parallax[] _Parallaxes;

    [SerializeField] Slider _FuelSlider;
    float _CurrentFuel;

    [SerializeField] float _FuelDecreaseRate;

    [SerializeField, Range(1f, 10f)] float _ExplodeIdleTime = 3f;
    float _CurrentIdleTime;

    CharacterController controller;
    Transform _PlayerTransform;
    SFXController _SFXController;

    bool _IsPlayerDead = false;
    bool _IsOutOfFuel = false;
    bool _IsMoving = false;

    int _InitialMove = 0;

    private void Awake()
    {
        _PlayerTransform = transform;

        controller = GetComponent<CharacterController>();
        _SFXController = GetComponentInChildren<SFXController>();

        // input actions
        _AccelerateAction = SpeedSeeker.Input.PlayerInputManager.Instance.FindAction("Move");

        Assert.IsNotNull(_SFXController);
        Assert.IsNotNull(_PlayerCamera);
        Assert.IsNotNull(_PlayerCollision);
    }

    private void Start()
    {
        _FuelSlider.maxValue = ship.stats.fuel;
        _CurrentFuel = ship.stats.fuel;
        _FuelSlider.SetValueWithoutNotify(_FuelSlider.maxValue);
    }

    private void Update()
    {
        ExplodeAfterIdleTime(_ExplodeIdleTime);
        if (_SpeedBoostDuration > 0)
        {
            ExecuteSpeedBoost();
        }
    }

    void FixedUpdate()
    {
        if (!_IsPlayerDead)
        {
            HandleInput();
            AddCashPerSpeedValue();
            _IsMoving = _CurrentSpeed > 0f;
        }
        else if (_CurrentSpeed > 0f)
        {
            DeAccelerate();
            ParallaxSpeed(_CurrentSpeed);
        }
    }

    private void DecreaseFuel()
    {        
        if (_IsOutOfFuel || GameManager.Instance.AdventureMode) return;

        if (_CurrentSpeed >= 1)
        {
            float decreaseRate = _FuelDecreaseRate;
            float decreaseAmount = (decreaseRate * _CurrentSpeed) * Time.deltaTime;

            _FuelSlider.value -= decreaseAmount;
            _CurrentFuel = _FuelSlider.value;
        }
        _IsOutOfFuel = _CurrentFuel <= 0;
    }

    /// <summary>
    /// Will update the Fuel slider to what ever the 'CurrentFuel' is
    /// </summary>
    public void UpdateFuelUI()
    {
        _FuelSlider.value = _CurrentFuel;
        _IsOutOfFuel = _CurrentFuel <= 0f;
    }

    public void HandleInput()
    {
        MoveShip();
        if (_AccelerateAction.IsPressed() && !_IsOutOfFuel) Accelerate();
        else DeAccelerate();
        ParallaxSpeed(_CurrentSpeed);
    }
    
    public void CreateExplosion()
    {
        if (ship.stats.hitPoints <= 0)
        {
            _SFXController.PlayRandomSoundEffect("Explosions");
            Explosion.CreateExplosion(ship.shipObject.ExplosionPrefab, _PlayerTransform);
        }
        _IsPlayerDead = true;
    }

    void ParallaxSpeed(float multiplier)
    {
        foreach (var parallax in _Parallaxes)
        {
            // really have to tone down the value on this because it can't handle any big numbers,
            // the parallax freaks out otherwise
            parallax.SetSpeed(multiplier*0.15f);
        }
    }   

    void Accelerate()
    {
        if (_InitialMove == 0) _InitialMove++;

                                        /* multiply by 5, a way of making the speed (parallax) look realistic to value of speed in meters/s */
        _CurrentSpeed = (int)Mathf.Lerp(_CurrentSpeed, ship.stats.speed*5f, (Time.fixedDeltaTime * ship.stats.acceleration));
        
        if (_SpeedBoostDuration > 0) _CurrentSpeed = Mathf.RoundToInt(_CurrentSpeed * _SpeedBoostMultiplier);
        else DecreaseFuel();        
        
        if (BestSpeed < _CurrentSpeed) BestSpeed = _CurrentSpeed;

        if (!ship.IsThrusterEngaged()) ship.EngageThruster();
    }

    void DeAccelerate()
    {
        _CurrentSpeed = (int)Mathf.Lerp(_CurrentSpeed, 0f, Time.fixedDeltaTime * 0.5f);
        if (ship.IsThrusterEngaged()) ship.DisEngageThruster();
    }

    public void ApplySpeedBoost(float multiplier, float duration)
    {        
        _SpeedBoostMultiplier += multiplier;
         _SpeedBoostDuration += duration;
    }

    void ExecuteSpeedBoost()
    {
        if (_CurrentSpeedBoostTime < _SpeedBoostDuration)
        {
            _CurrentSpeedBoostTime += Time.deltaTime;
            Accelerate();
        }

        if (_CurrentSpeedBoostTime >= _SpeedBoostDuration)
        {
            _CurrentSpeedBoostTime = 0;
            _SpeedBoostDuration = 0;
            _SpeedBoostMultiplier = 0;
        }
    }

    public void DoOnInvulnerableStateChange()
    {
        if (_PlayerCollision.IsInvulnerable) ship.Shield.SetActive(true);
        else ship.Shield.SetActive(false);
    }

    void ExplodeAfterIdleTime(float duration)
    {
        if (_IsPlayerDead) return;

        // only update if the player isn't moving and has atleast moved once
        if (!_IsMoving && _InitialMove != 0)
        {
            _CurrentIdleTime += Time.deltaTime;
            if (_CurrentIdleTime >= duration)
            {
                var playerCollisionComp = GetComponentInChildren<PlayerCollision>();
                if (playerCollisionComp != null) playerCollisionComp.onPlayerDie?.Invoke();
            }
        }
        else if (_IsMoving && _CurrentIdleTime > 0f)
        {
            _CurrentIdleTime = 0f;
        }
    }

    public void AddFuel(float amount)
    {
        _CurrentFuel += ship.stats.fuel * (amount / 100);
        _CurrentFuel = Mathf.Clamp(_CurrentFuel, 0f, ship.stats.fuel);
        UpdateFuelUI();
    }

    /// <summary>
    /// Only moves the ship from left to right based on mouse position.
    /// </summary>
    /// <remarks>
    /// Call Accelerate() to make the ship "move" forward.
    /// </remarks>
    void MoveShip()
    {
        Vector2 direction = GetMoveDirection();
        RotateTowardsMouse();
        if (direction.sqrMagnitude > 0f)
        {
            float tempSpeed = Mathf.Lerp(5f, ship.shipObject.HorizontalSpeed, Time.fixedDeltaTime * ship.stats.acceleration);
            direction.y = 0f;
            controller.Move(direction * (tempSpeed * Time.fixedDeltaTime));
        }        
    }

    void AddCashPerSpeedValue()
    {
        if (_CurrentSpeed < 1) return;
        GameManager.Instance.AddCash(_CurrentSpeed/6);
    }

    void RotateTowardsMouse()
    {       
        Vector2 direction = GetMoveDirection();
        float angle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
        _PlayerTransform.rotation = Quaternion.AngleAxis(-angle, Vector3.forward);
    }

    Vector2 GetMoveDirection()
    {
        Vector3 mousePosition = _PlayerCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mousePosition - _PlayerTransform.position).normalized;
        return direction;
    }
}
