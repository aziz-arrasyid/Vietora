using UnityEngine;
using PrimeTween;
using TMPro;

public class AnimatedNotification : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private RectTransform notification;
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI smallText;
    [Header("Value")]
    [SerializeField] private float durationAnimation;
    [SerializeField] private float durationDelay;
    private CanvasGroup cg;
    private PrimeTween.Sequence sequence;

    private void Start()
    {
        cg = notification.GetComponent<CanvasGroup>();

        ChangeLanguage();

        GameManager.instance.settings.OnLangChanged += ChangeLanguage;
    }

    private void ChangeLanguage()
    {
        Language lang = GameManager.instance.settings.Language;
        LocalizedNotification notifLang = GameManager.instance.localizedMainMenuText.notification;

        title.text = notifLang.title.GetText(lang);
        smallText.text = notifLang.smallText.GetText(lang);
    }

    public void Animated()
    {
        if (sequence.isAlive) sequence.Stop();

        sequence = Sequence.Create();

        sequence
        .ChainCallback(target: SoundManager.instance, target => target.PlaySFXSound(target.notifSound))
        .Group(Tween.UIAnchoredPositionY(target: notification, startValue: 100f, endValue: -300f, duration: durationAnimation, ease: Ease.OutSine))
        .Group(Tween.Alpha(target: cg, startValue: 0f, endValue: 1f, duration: durationAnimation, ease: Ease.OutSine))
        .ChainDelay(durationDelay)
        .Chain(Tween.Alpha(target: cg, endValue: 0f, duration: durationAnimation, ease: Ease.InSine));
    }
}
