using PrimeTween;
using UnityEngine;
using UnityEngine.UI;

public class AnimatedDropdown : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private Button toggleBtn;
    [SerializeField] private Button settingsBtn;
    [SerializeField] private Button closeBtn;
    [SerializeField] private RectTransform container;
    [SerializeField] private CanvasGroup containerCanvasGroup;
    [Space]
    [SerializeField] private SettingsController settingsController;
    [Space]
    [SerializeField] private float duration;

    private bool isOn;

    private void Start()
    {
        if (toggleBtn != null)
        {
            toggleBtn.onClick.AddListener(OnToggle);
        }

        if (settingsBtn != null && closeBtn != null)
        {
            settingsBtn.onClick.AddListener(OnSettingsbtnClicked);
            closeBtn.onClick.AddListener(OnCloseBtnClicked);
        }

        if (container != null)
        {
            container.gameObject.SetActive(false);

            container.localScale = new(1f, 0f, 1f);
            containerCanvasGroup.alpha = 0f;

        }

        if (settingsController != null)
        {
            settingsController.OnCloseBtnClicked += () => ClosePanel(settingsController.panel);
        }
    }

    private void InteractableBtn(bool value)
    {
        toggleBtn.interactable = value;
        settingsBtn.interactable = value;
        closeBtn.interactable = value;
    }

    private void OnToggle()
    {
        InteractableBtn(false);

        isOn = !isOn;

        Tween.StopAll(toggleBtn);
        if (containerCanvasGroup != null) Tween.StopAll(containerCanvasGroup);

        if (isOn) // Open
        {
            container.gameObject.SetActive(true);
            Tween.ScaleY(container, endValue: 1f, duration: duration, ease: Ease.OutQuad)
                .OnComplete(target: this, target => target.InteractableBtn(true));

            Tween.Alpha(containerCanvasGroup, endValue: 1f, duration: duration, ease: Ease.OutQuad);
        }
        else
        {
            Tween.ScaleY(container, endValue: 0f, duration: duration, ease: Ease.OutQuad)
                .OnComplete(target: this, target =>
                {
                    target.container.gameObject.SetActive(false);
                    target.InteractableBtn(true);
                });

            Tween.Alpha(containerCanvasGroup, endValue: 0f, duration: duration, ease: Ease.OutQuad);
        }
    }

    private void OnSettingsbtnClicked()
    {
        OnToggle();
        OpenPanel(settingsController.panel);
    }

    private void OnCloseBtnClicked() => OnToggle();

    private void OpenPanel(RectTransform panel)
    {
        panel.localScale = Vector3.zero;
        CanvasGroup canvasGroup = panel.GetComponent<CanvasGroup>();

        panel.gameObject.SetActive(true);

        canvasGroup.alpha = 0f;

        Tween.Alpha(canvasGroup, endValue: 1f, duration: 0.5f);
        Tween.Scale(panel, endValue: Vector3.one, duration: 0.5f, ease: Ease.OutBack);
    }

    private void ClosePanel(RectTransform panel)
    {
        CanvasGroup canvasGroup = panel.GetComponent<CanvasGroup>();

        Tween.Alpha(canvasGroup, endValue: 0f, duration: 0.5f);
        Tween.Scale(panel, endValue: Vector3.zero, duration: 0.5f, ease: Ease.InBack)
            .OnComplete(target: panel, target => target.gameObject.SetActive(false));
    }
}
