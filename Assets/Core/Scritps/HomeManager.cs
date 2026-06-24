using TMPro;
using UnityEngine;
using UnityEngine.UI;
using PrimeTween;

public enum VolumesType
{
    musicVolume,
    SFXVolume,
    narratorVolume
}

public class HomeManager : MonoBehaviour
{
    [SerializeField] private GameObject mainMenu;
    [Header("Button")]
    [SerializeField] private Button startBtn;
    [SerializeField] private Button settingsBtn;
    [SerializeField] private Button creditsBtn;
    [SerializeField] private Button exitBtn;
    [SerializeField] private Button closeBtnCredits;
    [SerializeField] private Button closeBtnSettings;
    [Space]
    [Header("Text Display")]
    [SerializeField] private TextMeshProUGUI startBtnText;
    [SerializeField] private TextMeshProUGUI settingsBtnText;
    [SerializeField] private TextMeshProUGUI creditsBtnText;
    [SerializeField] private TextMeshProUGUI exitBtnText;
    [SerializeField] private TextMeshProUGUI closeBtnText;
    [Space]
    [Header("Panel")]
    [SerializeField] private RectTransform creditsPanel;
    [SerializeField] private SettingsController settingsController; 

    private bool isPanelOpen;

    private void Start()
    {
        DisplayText();

        startBtn.onClick.AddListener(OnStartBtnClicked);
        settingsBtn.onClick.AddListener(OnSettingsBtnClicked);
        creditsBtn.onClick.AddListener(OnCreditsBtnClicked);
        exitBtn.onClick.AddListener(OnExitBtnClicked);
        closeBtnCredits.onClick.AddListener(OnCloseCreditsBtnClicked);

        settingsController.OnCloseBtnClicked += OnCloseSettingsBtnClicked;

        GameManager.instance.settings.OnLangChanged += DisplayText;
    }

    private void DisplayText()
    {
        Language lang = GameManager.instance.settings.Language;
        var textData = GameManager.instance.localizedMainMenuText;

        startBtnText.text = textData.startBtn.GetText(lang);
        settingsBtnText.text = textData.settingsBtn.GetText(lang);
        creditsBtnText.text = textData.creditsBtn.GetText(lang);
        exitBtnText.text = textData.exitBtn.GetText(lang);

    }

    #region Main Menu Button
    public void OnStartBtnClicked()
    {
        Debug.Log("Start button klik!");
    }

    public void OnSettingsBtnClicked()
    {
        OpenPanel(settingsController.panel);
    }

    public void OnCreditsBtnClicked()
    {
        OpenPanel(creditsPanel);
    }

    public void OnExitBtnClicked()
    {
        Debug.Log("Exit button klik!");
    }
    #endregion

    public void OnCloseCreditsBtnClicked()
    {
        ClosePanel(creditsPanel);
    }

    public void OnCloseSettingsBtnClicked()
    {
        ClosePanel(settingsController.panel);
    }

    private void OpenPanel(RectTransform panel)
    {
        if (isPanelOpen) return;
        isPanelOpen = true;

        panel.localScale = Vector2.zero;
        panel.GetComponent<CanvasGroup>().alpha = 0;
        panel.gameObject.SetActive(true);
        mainMenu.SetActive(false);

        ScrollRect scrollRect = panel.GetComponentInChildren<ScrollRect>();
        CanvasGroup canvasGroup = panel.GetComponent<CanvasGroup>();

        Tween.Alpha(canvasGroup, endValue: 1f, duration: 0.2f);

        var tweenScale = Tween.Scale(panel, endValue: Vector2.one, duration: 0.3f, ease: Ease.OutBack);

        if (scrollRect != null)
        {
            tweenScale.OnUpdate(target: scrollRect, (target, tween) => target.verticalNormalizedPosition = 1f);
        }
    }

    private void ClosePanel(RectTransform panel)
    {
        if (!isPanelOpen) return;
        isPanelOpen = false;

        Tween.Alpha(panel.GetComponent<CanvasGroup>(), endValue: 0f, duration: 0.2f);
        Tween.Scale(panel, endValue: Vector2.zero, duration: 0.2f, ease: Ease.InBack)
            .OnComplete(target: panel, target => target.gameObject.SetActive(false));

        mainMenu.SetActive(true);
    }
}
