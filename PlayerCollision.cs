using Sirenix.OdinInspector;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class PlayerCollision : MonoBehaviour
{
    [SerializeField, Required] Player _Player;
    [SerializeField, Required] SpriteRenderer _SpriteRenderer;
    [SerializeField, Required] TMP_Text _HealthValueText;
    [SerializeField, Required] Ship _Ship;

    BoxCollider2D _BoxCollider;
    CharacterController _PlayerController;
    public bool IsInvulnerable {get; private set;}

    public UnityEvent onPlayerDie = new UnityEvent();
    public UnityEvent onImpact = new UnityEvent();
    public UnityEvent<Collectable.CollectablesType> onCollectablePickup = new UnityEvent<Collectable.CollectablesType>();
    public UnityEvent onInvulnerableStateChange = new UnityEvent();

    Coroutine _MagnetCoroutine;
    Coroutine _ShieldCoroutine;

    void Start()
    {
        _BoxCollider = GetComponent<BoxCollider2D>();
        _PlayerController = _Player.GetComponent<CharacterController>();
        CalculateCollisionSize();
        _HealthValueText.text = _Ship.stats.hitPoints.ToString();
    }

    void CalculateCollisionSize()
    {
        Vector2 spriteBounds = _SpriteRenderer.bounds.size;
        _PlayerController.radius = spriteBounds.x / 2f;
        _PlayerController.height = spriteBounds.y;

        spriteBounds = new Vector2(_SpriteRenderer.bounds.size.x /2, _SpriteRenderer.bounds.size.y);
        _BoxCollider.size = spriteBounds;
    }

    void TakeDamage(int damage)
    {
        _Ship.TakeDamage(damage);
        if (_Ship.stats.hitPoints <= 0)
        {
            onPlayerDie?.Invoke();
            //Destroy(gameObject);
            gameObject.SetActive(false);
        }

        _HealthValueText.text = _Ship.stats.hitPoints.ToString();
    }

    bool ShouldTakeDamage(Obstacle obstacle)
    {
        if (!IsInvulnerable && obstacle.LastVelocity.sqrMagnitude > 1f && obstacle.NumberOfImpactsOnPlayer < 1) return true;

        return false;
    }


    void OnCollisionEnter2D(Collision2D other)
    {
        // check if it or its parent is an 'obstacle'
        Obstacle obstacle = other.gameObject.GetComponentInParent<Obstacle>();
        if (obstacle)
        {
            HandleObstacleCollision(obstacle);
            return;
        }

        // check if it or its parent is a 'collectable'
        Collectable collectable = other.gameObject.GetComponentInParent<Collectable>();
        if (collectable)
        {
            HandleCollectableCollision(collectable);
            return;
        }

        Debug.LogWarning("Couldn't find any of the required components (either Obstacle or Collectable).");
    }

    private void HandleCollectableCollision(Collectable collectable)
    {
        switch (collectable.CollectableType)
        {
            case Collectable.CollectablesType.Health:
                _Ship.AddHealth(collectable.HealthIncrease);
                _HealthValueText.text = _Ship.stats.hitPoints.ToString();
                break;

            case Collectable.CollectablesType.SpeedBoost:
                _Player.ApplySpeedBoost(collectable.SpeedMultiplier, collectable.SpeedIncreaseDuration);                
                break;

            case Collectable.CollectablesType.Fuel:
                _Player.AddFuel(collectable.FuelIncrease);
                break;

            case Collectable.CollectablesType.Cash:
                GameManager.Instance.AddCash(collectable.CashIncrease);
                break;

            case Collectable.CollectablesType.Shield:
                if (_ShieldCoroutine != null) StopCoroutine(_ShieldCoroutine);
                IsInvulnerable = true;
                onInvulnerableStateChange?.Invoke();
                _ShieldCoroutine = StartCoroutine(StartShieldTimer(collectable.ShieldDuration));
                break;

            case Collectable.CollectablesType.Magnet:
                if (!TryGetComponent(out Attractor magnet))
                {
                    magnet = gameObject.AddComponent<Attractor>();
                    magnet.AttractorRadius = collectable.MagnetRadius;
                    magnet.AttractorStrength = collectable.MagnetStrength;
                    magnet.AttractorPoint = transform;                    
                }
                else
                {
                    magnet.AttractorRadius = collectable.MagnetRadius;
                    magnet.AttractorStrength = collectable.MagnetStrength;
                    magnet.AttractorPoint = transform;
                }
                magnet.AffectedLayers = 1 << PhysicsLayer.Collectable + 1;
                if (_MagnetCoroutine != null) StopCoroutine(_MagnetCoroutine);
                TryGetComponent(out CircleCollider2D collider);
                _MagnetCoroutine = StartCoroutine(StartMagnetTimer(collectable.MagnetDuration, magnet, collider));

                break;

            default: break;
        }
        collectable.ReturnToPool();

        onCollectablePickup?.Invoke(collectable.CollectableType);
    }

    IEnumerator StartShieldTimer(float duration)
    {
        yield return new WaitForSeconds(duration);
        IsInvulnerable = false;
        onInvulnerableStateChange?.Invoke();
    }

    IEnumerator StartMagnetTimer(float duration, Attractor magnet, CircleCollider2D collider)
    {
        magnet.enabled = true;
        collider.enabled = true;
        yield return new WaitForSeconds(duration);
        magnet.enabled = false;
        collider.enabled = false;
    }

    private void HandleObstacleCollision(Obstacle obstacle)
    {
        int damage = obstacle.ObstacleDamage;

        // if the obstacle isn't moving fast enough take no damage, this doesn't take in account for the players velocity
        if (ShouldTakeDamage(obstacle))
        {
            TakeDamage(damage);
        }
        onImpact?.Invoke();

        obstacle.IncreaseImpactsOnPlayer();
    }

#if UNITY_EDITOR
    [Button]
    void SetInvulnerable(bool value)
    {
        IsInvulnerable = value;
        onInvulnerableStateChange?.Invoke();
    }
#endif
}
