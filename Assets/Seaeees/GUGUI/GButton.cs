using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Seaeees.GUGUI.Tween;

namespace Seaeees.GUGUI
{
    public class GButton : MonoBehaviour , IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {
        private Coroutine _scaleAnimationCoroutine;
        private Coroutine _colorAnimationCoroutine;
        private Coroutine _fillAnimationCoroutine;
        private RectTransform _rectTransform;
        private Image _image;
        private Image _fillImage;
        private Color _defaultColor;
        private Vector2 _defaultScale;
        private Sprite _defaultSprite;
        private Vector2 _calculatedScaleOnHover;
        private Vector2 _calculatedScaleOnClick;

        [SerializeField] private bool useScaleAnimationOnHover;
        [SerializeField] private bool useScaleAnimationOnClick;
        [SerializeField] private bool useColorAnimationOnHover;
        [SerializeField] private bool useColorAnimationOnClick;
        [SerializeField] private bool useAudioPlayer;
        [SerializeField] private bool useFillAmountAnimation;
        [SerializeField] private bool useImageChangerOnHover;
        [SerializeField] private bool useImageChangerOnClick;
        [SerializeField] private EaseType scaleEaseType = EaseType.Linear;
        [SerializeField] private Vector2 scaleOnHover;
        [SerializeField] private float scaleDurationOnHover = 0.1f;
        [SerializeField] private Vector2 scaleOnClick;
        [SerializeField] private float scaleDurationOnClick = 0.1f;
        [SerializeField] private Color colorOnHover = Color.gray;
        [SerializeField] private float colorDurationOnHover = 0.1f;
        [SerializeField] private Color colorOnClick = Color.gray;
        [SerializeField] private float colorDurationOnClick = 0.1f;
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip hoverEnterAudioClip;
        [SerializeField] private AudioClip hoverExitAudioClip;
        [SerializeField] private AudioClip downAudioClip;
        [SerializeField] private AudioClip upAudioClip;
        [SerializeField] private EaseType fillImageEaseType = EaseType.Linear;
        [SerializeField] private Image fillImage;
        [SerializeField] private float fillImageDuration = 0.1f;
        [SerializeField] private Sprite hoverImage;
        [SerializeField] private Sprite clickImage;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _image = GetComponent<Image>();
            
            _fillImage = fillImage;
            _defaultScale = _rectTransform.sizeDelta;
            _defaultColor = _image.color;
            _defaultSprite = _image.sprite;
            
            _calculatedScaleOnHover = new Vector2(_defaultScale.x + scaleOnHover.x, _defaultScale.y + scaleOnHover.y);
            _calculatedScaleOnClick = new Vector2(_defaultScale.x + scaleOnClick.x, _defaultScale.y + scaleOnClick.y);
        }

        public void OnPointerEnter(PointerEventData eventData) => PlayButtonEffects(AnimationType.PointerEnter);

        public void OnPointerExit(PointerEventData eventData) => PlayButtonEffects(AnimationType.PointerExit);

        public void OnPointerDown(PointerEventData eventData) => PlayButtonEffects(AnimationType.PointerDown);

        public void OnPointerUp(PointerEventData eventData) => PlayButtonEffects(AnimationType.PointerUp);
        
        private void PlayButtonEffects(AnimationType type)
        {
            PlayAudio(type);
            AnimationFillAmount(type);
            AnimationScale(type);
            AnimationColor(type);
            ImageChange(type);
            
            //TODO:クリック後の動作
            if(type == AnimationType.PointerUp) PlayButtonEffects(AnimationType.PointerEnter);
        }
        
        private void PlayAudio(AnimationType animationType)
        {
            if (!audioSource) return;
            if (!useAudioPlayer) return;
            if (animationType == AnimationType.PointerEnter)
                audioSource.PlayOneShot(hoverEnterAudioClip);
            else if (animationType == AnimationType.PointerExit)
                audioSource.PlayOneShot(hoverExitAudioClip);
            else if (animationType == AnimationType.PointerDown)
                audioSource.PlayOneShot(downAudioClip);
            else if (animationType == AnimationType.PointerUp) 
                audioSource.PlayOneShot(upAudioClip);
        }
        
        private void AnimationFillAmount(AnimationType animationType)
        {
            if (!_fillImage) return;
            if(!useFillAmountAnimation) return;
            if (animationType == AnimationType.PointerEnter)
                ResetCoroutine(ref _fillAnimationCoroutine, _fillImage.AnimateFillAmount(1, fillImageDuration, fillImageEaseType));
            else if (animationType == AnimationType.PointerExit)
                ResetCoroutine(ref _fillAnimationCoroutine, _fillImage.AnimateFillAmount(0, fillImageDuration, fillImageEaseType));
        }
        
        private void AnimationScale(AnimationType animationType)
        {
            if (animationType == AnimationType.PointerEnter && useScaleAnimationOnHover)
                ResetCoroutine(ref _scaleAnimationCoroutine, _rectTransform.AnimateScale(_calculatedScaleOnHover, scaleDurationOnHover, scaleEaseType));
            else if (animationType == AnimationType.PointerExit && useScaleAnimationOnHover)
                ResetCoroutine(ref _scaleAnimationCoroutine, _rectTransform.AnimateScale(_defaultScale, scaleDurationOnHover, scaleEaseType));
            else if (animationType == AnimationType.PointerDown && useScaleAnimationOnClick)
                ResetCoroutine(ref _scaleAnimationCoroutine, _rectTransform.AnimateScale(_calculatedScaleOnClick, scaleDurationOnClick, scaleEaseType));
            else if (animationType == AnimationType.PointerUp && useScaleAnimationOnClick)
                ResetCoroutine(ref _scaleAnimationCoroutine, _rectTransform.AnimateScale(_defaultScale, scaleDurationOnClick, scaleEaseType));
        }
        
        private void AnimationColor(AnimationType animationType)
        {
            if (animationType == AnimationType.PointerEnter && useColorAnimationOnHover)
                ResetCoroutine(ref _colorAnimationCoroutine, _image.AnimateColor(colorOnHover, colorDurationOnHover));
            else if (animationType == AnimationType.PointerExit && useColorAnimationOnHover)
                ResetCoroutine(ref _colorAnimationCoroutine, _image.AnimateColor(_defaultColor, colorDurationOnHover));
            else if (animationType == AnimationType.PointerDown && useColorAnimationOnClick)
                ResetCoroutine(ref _colorAnimationCoroutine, _image.AnimateColor(colorOnClick, colorDurationOnClick));
            else if (animationType == AnimationType.PointerUp && useColorAnimationOnClick)
                ResetCoroutine(ref _colorAnimationCoroutine, _image.AnimateColor(_defaultColor, colorDurationOnClick));
        }
        
        private void ImageChange(AnimationType animationType)
        {
            if (animationType == AnimationType.PointerEnter && useImageChangerOnHover)
                _image.sprite = hoverImage;
            else if (animationType == AnimationType.PointerExit && useImageChangerOnHover)
                _image.sprite = _defaultSprite;
            else if (animationType == AnimationType.PointerDown && useImageChangerOnClick)
                _image.sprite = clickImage;
            else if (animationType == AnimationType.PointerUp && useImageChangerOnClick) 
                _image.sprite = _defaultSprite;
        }
        
        private void ResetCoroutine(ref Coroutine coroutine, IEnumerator enumerator)
        {
            if (coroutine != null) StopCoroutine(coroutine);
            coroutine = StartCoroutine(enumerator);
        }
    }
}
