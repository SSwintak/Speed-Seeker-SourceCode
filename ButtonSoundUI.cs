
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;

//[RequireComponent(typeof(Button))]
public class ButtonSoundUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{

    private enum PlaySoundMode
    { 
        OnEnter,
        OnExit,
        OnClick,
        All
    }
    [Tooltip("The audio source will be created on this object, does NOT duplicate instead will use the same audio source. If left empty the source will be created on the attached object.")]
    [SerializeField, Required] AudioSource _UISoundPlayer;

    [SerializeField, EnumToggleButtons] PlaySoundMode playMode;
    [SerializeField, HideIf("playMode", PlaySoundMode.All)] AudioClip sound;

    [ShowIfGroup("playMode", PlaySoundMode.All)]
    [BoxGroup("playMode/All Sounds")]
    [SerializeField] AudioClip onEnterSound, onExitSound, onClickSound;

    static AudioSource source;

    private void Start()
    {
        if (!source) source = _UISoundPlayer;
        source.clip = sound;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (playMode == PlaySoundMode.OnEnter) PlaySound();
        else if (playMode == PlaySoundMode.All)
        {
            source.clip = onEnterSound;
            PlaySound();
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (playMode == PlaySoundMode.OnExit) PlaySound();
        else if (playMode == PlaySoundMode.All)
        {
            source.clip = onExitSound;
            PlaySound();
        }
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (playMode == PlaySoundMode.OnClick) PlaySound();
        else if (playMode == PlaySoundMode.All)
        {
            source.clip = onClickSound;
            PlaySound();
        }
    }

    private void PlaySound()
    {
        if (!source || source.clip == null) return;
        source.Play();
    }

}
