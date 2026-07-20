using PrimeTween;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class VisibilityController : MonoBehaviour
{
    [SerializeField] private QuizzManager quizzManager;
    [Header("UI Components")]
    [SerializeField] private GameObject dropdown;
    [SerializeField] private GameObject dialogue;
    [SerializeField] private GameObject quizzBtn;
    [SerializeField] private Button visibilityBtn;

    [Header("Value")]
    [Range(0, 1)]
    [SerializeField] private float visibleDuration;

    private float targetOtherVisible;
    private float targetOwnVisible;
    public bool Visibility { get; private set; } = true;

    private CanvasGroup canvasGroupDropdown;
    private CanvasGroup canvasGroupVisibilityBtn;
    private CanvasGroup canvasGroupDialogue;
    private CanvasGroup canvasGroupQuizzBtn;
    private Image visibilityImage;
    private PrimeTween.Sequence sequence;
    private Tween transitionSprite;
    private AnimatedDialogue animDialogue;

    private void Awake()
    {
        if (dropdown == null || visibilityBtn == null || dialogue == null) return;

        canvasGroupDropdown = dropdown.GetComponent<CanvasGroup>();
        canvasGroupVisibilityBtn = visibilityBtn.GetComponent<CanvasGroup>();
        canvasGroupDialogue = dialogue.GetComponent<CanvasGroup>();
        canvasGroupQuizzBtn = quizzBtn.GetComponent<CanvasGroup>();

        visibilityImage = visibilityBtn.GetComponent<Image>();

        animDialogue = dialogue.GetComponent<AnimatedDialogue>();
    }

    private void OnEnable() => visibilityBtn.onClick.AddListener(OnToggle);
    private void OnDisable() => visibilityBtn.onClick.RemoveListener(OnToggle);

    private void OnToggle()
    {
        Visibility = !Visibility;

        if (sequence.isAlive) sequence.Stop();

        sequence = PrimeTween.Sequence.Create();

        targetOtherVisible = Visibility == true ? 1f : 0f;
        targetOwnVisible = Visibility == true ? 1f : 0.2f;

        if (Visibility)
        {
            dropdown.SetActive(true);
            dialogue.SetActive(true);

            if (quizzManager.quizzModelData.alreadyCompleted)
            {
                quizzBtn.SetActive(true);
            }
        }

        ChangeSpriteVisibleBtn(Visibility);

        SoundManager.instance.PlaySFXSound(SoundManager.instance.clickSound);
        sequence
        .Group(Tween.Alpha(canvasGroupDropdown, endValue: targetOtherVisible, duration: visibleDuration, ease: Ease.InOutSine))
        .Group(Tween.Alpha(canvasGroupVisibilityBtn, endValue: targetOwnVisible, duration: visibleDuration, ease: Ease.InOutSine));

        if (quizzManager.quizzModelData.alreadyCompleted)
        {
            sequence.Group(Tween.Alpha(canvasGroupQuizzBtn, endValue: targetOwnVisible, duration: visibleDuration, ease: Ease.InOutSine));
        }

        if (canvasGroupDialogue != null && dialogue.activeSelf)
        {
            if (canvasGroupDialogue.alpha != targetOtherVisible)
            {
                sequence.Group(Tween.Alpha(canvasGroupDialogue, endValue: targetOtherVisible, duration: visibleDuration, ease: Ease.InOutSine));
            }
        }

        if (!Visibility)
        {
            sequence.OnComplete(target: this, target =>
            {
                target.dropdown.SetActive(false);
                target.dialogue.SetActive(false);

                if (quizzManager.quizzModelData.alreadyCompleted)
                {
                    target.quizzBtn.SetActive(false);
                }
            });
        }
    }

    private void ChangeSpriteVisibleBtn(bool visible)
    {
        float startValue = visibilityImage.material.GetFloat("_Amount");
        float endValue = visible ? 1f : 0f;

        if (transitionSprite.isAlive) transitionSprite.Stop();

        transitionSprite = Tween.Custom(
            target: visibilityImage.material,
            startValue: startValue,
            endValue: endValue,
            duration: visibleDuration,
            onValueChange: (target, value) =>
            {
                target.SetFloat("_Amount", value);
            }

        );
    }
}
