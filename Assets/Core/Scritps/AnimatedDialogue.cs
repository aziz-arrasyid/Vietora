using PrimeTween;
using UnityEngine;
using UnityEngine.UI;

public class AnimatedDialogue : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private RectTransform dialogue;
    [SerializeField] private RectTransform logChatBtn;
    [SerializeField] private RectTransform dialogueBox;
    [SerializeField] private Button hideSeeBtn;

    [Space]

    [Header("Value")]
    [Range(0, 1)]
    [SerializeField] private float slideDuration;
    [Range(0, 1)]
    [SerializeField] private float alphaDuration;

    private CanvasGroup canvasGroupLogChatBtn;
    private CanvasGroup canvasGroupDialogueBox;
    private Sequence anim;
    private bool slideGoDown;
    private float targetPosY;
    private float targetAlpha;
    private Vector3 targetRotHideSeeBtn;
    private Vector3 startRotHideSeeBtn;
    private Ease targetEase;

    private void Awake()
    {
        if (logChatBtn == null || dialogueBox == null) return;

        canvasGroupLogChatBtn = logChatBtn.GetComponent<CanvasGroup>();
        canvasGroupDialogueBox = dialogueBox.GetComponent<CanvasGroup>();

        canvasGroupDialogueBox.alpha = 0f;
        canvasGroupLogChatBtn.alpha = 0f;
    }

    private void OnEnable() => hideSeeBtn.onClick.AddListener(AnimatedSlide);

    private void OnDisable() => hideSeeBtn.onClick.RemoveListener(AnimatedSlide);

    private void AnimatedSlide()
    {
        slideGoDown = !slideGoDown;

        if (slideGoDown)
        {
            targetPosY = -330f;
            targetAlpha = 0f;
            startRotHideSeeBtn = new(0, 0, 0);
            targetRotHideSeeBtn = new(0, 0, 60);
            targetEase = Ease.InSine;
        }
        else
        {
            targetPosY = 127f;
            targetAlpha = 1f;
            startRotHideSeeBtn = new(0, 0, 60);
            targetRotHideSeeBtn = new(0, 0, 0);
            targetEase = Ease.OutSine;
        }

        if (anim.isAlive) anim.Stop();

        anim = Sequence.Create();

        anim
        .Group(Tween.UIAnchoredPositionY(dialogue, endValue: targetPosY, duration: slideDuration, ease: targetEase))
        .Group(Tween.LocalEulerAngles(hideSeeBtn.transform, startValue: startRotHideSeeBtn, endValue: targetRotHideSeeBtn, duration: slideDuration, ease: targetEase))
        .Group(Tween.Alpha(canvasGroupLogChatBtn, endValue: targetAlpha, duration: alphaDuration, ease: targetEase))
        .Group(Tween.Alpha(canvasGroupDialogueBox, endValue: targetAlpha, duration: alphaDuration, ease: targetEase));
    }

    public void SetAnimatedGoDown(bool value)
    {
        slideGoDown = value;

        if (slideGoDown)
        {
            targetPosY = -450f;
            targetAlpha = 0f;
            startRotHideSeeBtn = new(0, 0, 0);
            targetRotHideSeeBtn = new(0, 0, 60);
            targetEase = Ease.InSine;
        }
        else
        {
            dialogue.gameObject.SetActive(true);
            targetPosY = 127f;
            targetAlpha = 1f;
            startRotHideSeeBtn = new(0, 0, 60);
            targetRotHideSeeBtn = new(0, 0, 0);
            targetEase = Ease.OutSine;
        }

        anim = Sequence.Create();

        anim
        .Group(Tween.UIAnchoredPositionY(dialogue, endValue: targetPosY, duration: slideDuration, ease: targetEase))
        .Group(Tween.LocalEulerAngles(hideSeeBtn.transform, startValue: startRotHideSeeBtn, endValue: targetRotHideSeeBtn, duration: slideDuration, ease: targetEase))
        .Group(Tween.Alpha(canvasGroupLogChatBtn, endValue: targetAlpha, duration: alphaDuration, ease: targetEase))
        .Group(Tween.Alpha(canvasGroupDialogueBox, endValue: targetAlpha, duration: alphaDuration, ease: targetEase));
    
        if(slideGoDown)
        {
            anim.OnComplete(target: dialogue, target => target.gameObject.SetActive(false));
        }
    }
}
