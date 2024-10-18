using Sirenix.OdinInspector;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Parallax : MonoBehaviour
{
    protected enum ParallaxScroll
    {
        Left,
        Right,
        Up,
        Down
    }

    [SerializeField] float _ParallaxSpeed;
    [SerializeField] ParallaxScroll _ScrollDirection;
    [SerializeField] bool _UsePixelDimensions = true;
    [SerializeField, HideIf("_UsePixelDimensions")] Vector2 _SpriteDimensions = Vector2.one;

    float _SingleTextureWidth;
    float _SingleTextureHeight;
    Vector2 _OriginalPosition;

    public float DefaultSpeed { get; private set; }

    void Start()
    {
        _OriginalPosition = new Vector2(transform.position.x, transform.position.y);

        SetupTexture();

        switch (_ScrollDirection)
        {
            case ParallaxScroll.Left:
                _ParallaxSpeed = -_ParallaxSpeed;
                break;

            case ParallaxScroll.Down:
                _ParallaxSpeed = -_ParallaxSpeed;
                break;

            default:
                break;
        }

        DefaultSpeed = _ParallaxSpeed;
    }
    
    void Update()
    {        
        CheckReset();
    }

    void FixedUpdate() 
    {
        ExecuteScroll();
    }

    public void SetSpeed(float multiplier)
    {
        if (multiplier == 0) _ParallaxSpeed = DefaultSpeed;
        else _ParallaxSpeed = -multiplier;

        _ParallaxSpeed = Mathf.Clamp(_ParallaxSpeed, -88f, 88f); // the max speed for parallax, anything higher will cause it to look like it's clipping
    }

    void ExecuteScroll()
    {
        float delta = _ParallaxSpeed * Time.fixedDeltaTime;
        Vector3 newPosition = new();
        switch (_ScrollDirection)
        {
            case ParallaxScroll.Left:
                newPosition = new Vector3(delta, 0f, 0f);
                break;

            case ParallaxScroll.Right:
                newPosition = new Vector3(delta, 0f, 0f);
                break;

            case ParallaxScroll.Up:
                newPosition = new Vector3(0f, delta, 0f);
                break;

            case ParallaxScroll.Down:
                newPosition = new Vector3(0f, delta, 0f);
                break;

            default:
                break;
        }
        transform.position += newPosition;
    }

    void CheckReset()
    {
        Transform trans = transform;
        if ((_ScrollDirection == ParallaxScroll.Up || _ScrollDirection == ParallaxScroll.Down) 
           && (Mathf.Abs(trans.position.y) - _SingleTextureHeight) > 0)
        {            
            trans.position = new Vector3(_OriginalPosition.x, _OriginalPosition.y, trans.position.z);
        }
        else if ((_ScrollDirection == ParallaxScroll.Left || _ScrollDirection == ParallaxScroll.Right)
           && (Mathf.Abs(trans.position.x) - _SingleTextureWidth) > 0)
        {
            trans.position = new Vector3(_OriginalPosition.x, _OriginalPosition.y, trans.position.z);
        }
    }

    void SetupTexture()
    {
        Sprite sprite = GetComponent<SpriteRenderer>().sprite;
        if (_UsePixelDimensions)
        {
            _SingleTextureWidth = sprite.texture.width / sprite.pixelsPerUnit;
            _SingleTextureHeight = sprite.texture.height / sprite.pixelsPerUnit; 
        }
        else
        {
            _SingleTextureWidth = _SpriteDimensions.x;
            _SingleTextureHeight = _SpriteDimensions.y;
        }
    }
}
