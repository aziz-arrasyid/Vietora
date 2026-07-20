using TMPro;
using UnityEngine;
using UnityEngine.UI;
using PrimeTween;

public class HomeManager : MonoBehaviour
{
    [SerializeField] private GameObject gameLogo;
    [SerializeField] private GameObject mainMenu;
    [Header("Button")]
    [SerializeField] private Button startBtn;
    [SerializeField] private Button settingsBtn;
    [SerializeField] private Button creditsBtn;
    [SerializeField] private Button exitBtn;
    [SerializeField] private Button closeBtnCredits;
    [SerializeField] private Button closeBtnSettings;
    [SerializeField] private Button noBtnConfirmExit;
    [SerializeField] private Button yesBtnConfirmExit;
    [Space]
    [Header("Text Display")]
    [SerializeField] private TextMeshProUGUI startBtnText;
    [SerializeField] private TextMeshProUGUI settingsBtnText;
    [SerializeField] private TextMeshProUGUI creditsBtnText;
    [SerializeField] private TextMeshProUGUI exitBtnText;
    [SerializeField] private TextMeshProUGUI closeBtnText;
    [SerializeField] private TextMeshProUGUI yesBtnConfirmExitText;
    [SerializeField] private TextMeshProUGUI noBtnConfirmExitText;
    [SerializeField] private TextMeshProUGUI questionConfirmExitText;
    [Space]
    [Header("Panel")]
    [SerializeField] private RectTransform creditsPanel;
    [SerializeField] private SettingsController settingsController;
    [SerializeField] private RectTransform exitPanel;

    private bool isPanelOpen;

    private void Start()
    {
        DisplayText();

        startBtn.onClick.AddListener(OnStartBtnClicked);
        settingsBtn.onClick.AddListener(OnSettingsBtnClicked);
        creditsBtn.onClick.AddListener(OnCreditsBtnClicked);
        exitBtn.onClick.AddListener(OnExitBtnClicked);
        closeBtnCredits.onClick.AddListener(OnCloseCreditsBtnClicked);
        noBtnConfirmExit.onClick.AddListener(OnNoBtnConfirmExit);
        yesBtnConfirmExit.onClick.AddListener(OnYesBtnConfirmBtn);

        settingsController.OnCloseBtnClicked += OnCloseSettingsBtnClicked;

        GameManager.instance.settings.OnLangChanged += DisplayText;

        SoundManager.instance.ChangeBGM(SoundManager.instance.bgmHome);

        if (SoundManager.instance.narratorSource.clip != null)
        {
            SoundManager.instance.narratorSource.Stop();
            SoundManager.instance.narratorSource.clip = null;
        }
    }

    private void DisplayText()
    {
        Language lang = GameManager.instance.settings.Language;
        var textData = GameManager.instance.localizedMainMenuText;

        startBtnText.text = textData.startBtn.GetText(lang);
        settingsBtnText.text = textData.settingsBtn.GetText(lang);
        creditsBtnText.text = textData.creditsBtn.GetText(lang);
        exitBtnText.text = textData.exitBtn.GetText(lang);
        yesBtnConfirmExitText.text = textData.exitPanel.yesBtn.GetText(lang);
        noBtnConfirmExitText.text = textData.exitPanel.noBtn.GetText(lang);
        questionConfirmExitText.text = textData.exitPanel.question.GetText(lang);

    }

    #region Main Menu Button
    public void OnStartBtnClicked()
    {
        SoundManager.instance.PlaySFXSound(SoundManager.instance.clickSound);
        SceneTransitionManager.instance.ChangeScene("World");
    }

    public void OnSettingsBtnClicked()
    {
        SoundManager.instance.PlaySFXSound(SoundManager.instance.clickSound);
        OpenPanel(settingsController.panel);
    }

    public void OnCreditsBtnClicked()
    {
        SoundManager.instance.PlaySFXSound(SoundManager.instance.clickSound);
        OpenPanel(creditsPanel);
    }

    public void OnExitBtnClicked()
    {
        SoundManager.instance.PlaySFXSound(SoundManager.instance.clickSound);
        OpenPanel(exitPanel);
    }
    #endregion

    public void OnCloseCreditsBtnClicked()
    {
        SoundManager.instance.PlaySFXSound(SoundManager.instance.clickSound);
        ClosePanel(creditsPanel);
    }

    public void OnCloseSettingsBtnClicked()
    {
        SoundManager.instance.PlaySFXSound(SoundManager.instance.clickSound);
        ClosePanel(settingsController.panel);
    }

    public void OnNoBtnConfirmExit()
    {
        SoundManager.instance.PlaySFXSound(SoundManager.instance.clickSound);
        ClosePanel(exitPanel);
    }

    public void OnYesBtnConfirmBtn()
    {
        SoundManager.instance.PlaySFXSound(SoundManager.instance.clickSound);
        OnExitGame();
    }

    private void OnExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif

        Application.Quit();
    }

    private void OpenPanel(RectTransform panel)
    {
        if (isPanelOpen) return;
        isPanelOpen = true;

        panel.localScale = Vector2.zero;
        panel.GetComponent<CanvasGroup>().alpha = 0;
        panel.gameObject.SetActive(true);
        mainMenu.SetActive(false);
        gameLogo.SetActive(false);

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
        gameLogo.SetActive(true);
    }
}
