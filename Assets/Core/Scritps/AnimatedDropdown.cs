using PrimeTween;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AnimatedDropdown : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private Button toggleBtn;
    [SerializeField] private Button settingsBtn;
    [SerializeField] private Button closeBtn;
    [SerializeField] private Button homeBtn;
    [SerializeField] private Button homeNoBtn;
    [SerializeField] private Button homeYesBtn;

    [SerializeField] private TextMeshProUGUI homeQuestionText;
    [SerializeField] private RectTransform toggleBtnIcon;
    [SerializeField] private RectTransform container;
    [SerializeField] private RectTransform homePanel;
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

        if (settingsBtn != null && closeBtn != null && homeBtn != null)
        {
            settingsBtn.onClick.AddListener(OnSettingsbtnClicked);
            closeBtn.onClick.AddListener(OnCloseBtnClicked);
            homeBtn.onClick.AddListener(OnHomeBtnClicked);
            homeNoBtn.onClick.AddListener(OnHomeNoBtnClicked);
            homeYesBtn.onClick.AddListener(OnHomeYesBtnClicked);
        }

        if (container != null)
        {
            container.gameObject.SetActive(false);

            container.localScale = new(1f, 0f, 1f);
            containerCanvasGroup.alpha = 0f;

        }

        if (settingsController != null)
        {
            settingsController.OnCloseBtnClicked += () => OnCloseBtnSettingsClicked();
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
        SoundManager.instance.PlaySFXSound(SoundManager.instance.clickSound);
        InteractableBtn(false);

        isOn = !isOn;
        Vector3 startRot = Vector3.zero;
        Vector3 endRot = Vector3.zero;

        Tween.StopAll(toggleBtn);
        if (containerCanvasGroup != null) Tween.StopAll(containerCanvasGroup);

        if (isOn) // Open
        {
            startRot.z = 0f;
            endRot.z = -180f;

            container.gameObject.SetActive(true);
            Tween.ScaleY(container, endValue: 1f, duration: duration, ease: Ease.OutQuad)
                .OnComplete(target: this, target => target.InteractableBtn(true));

            Tween.Alpha(containerCanvasGroup, endValue: 1f, duration: duration, ease: Ease.OutQuad);

        }
        else
        {
            startRot.z = -180f;
            endRot.z = 0f;

            Tween.ScaleY(container, endValue: 0f, duration: duration, ease: Ease.OutQuad)
                .OnComplete(target: this, target =>
                {
                    target.container.gameObject.SetActive(false);
                    target.InteractableBtn(true);
                });

            Tween.Alpha(containerCanvasGroup, endValue: 0f, duration: duration, ease: Ease.OutQuad);
        }

        Tween.LocalEulerAngles(target: toggleBtnIcon, startValue: startRot, endValue: endRot, duration: duration, ease: Ease.InOutSine);
    }

    private void OnSettingsbtnClicked()
    {
        OnToggle();
        OpenPanel(settingsController.panel);
    }

    private void OnCloseBtnSettingsClicked()
    {
        SoundManager.instance.PlaySFXSound(SoundManager.instance.clickSound);
        ClosePanel(settingsController.panel);
    }

    private void OnHomeBtnClicked()
    {
        OnToggle();
        OpenPanel(homePanel);
        LocalizedHomePanel localizedHomePanel = GameManager.instance.localizedMainMenuText.homePanel;
        bool langEN = GameManager.instance.settings.Language == Language.en;

        homeQuestionText.text = langEN ? localizedHomePanel.question.en : localizedHomePanel.question.id;
        homeYesBtn.GetComponentInChildren<TextMeshProUGUI>().text = langEN ? localizedHomePanel.yesBtn.en : localizedHomePanel.yesBtn.id;
        homeNoBtn.GetComponentInChildren<TextMeshProUGUI>().text = langEN ? localizedHomePanel.noBtn.en : localizedHomePanel.noBtn.id;
    }

    private void OnHomeNoBtnClicked()
    {
        SoundManager.instance.PlaySFXSound(SoundManager.instance.clickSound);
        ClosePanel(homePanel);
    }

    private void OnHomeYesBtnClicked()
    {
        SoundManager.instance.PlaySFXSound(SoundManager.instance.clickSound);
        SceneTransitionManager.instance.ChangeScene("MainMenu");
    }

    private void OnCloseBtnClicked()
    {
        OnToggle();
    }

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
