using System;
using PrimeTween;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsController : MonoBehaviour
{
    public Action OnCloseBtnClicked;
    
    [Header("UI Components")]
    public RectTransform panel;
    [SerializeField] private RectTransform toggleHandle;
    [SerializeField] private Image bgImageToggle;
    [SerializeField] private Image languageFlags;
    [Space]
    [Header("Button")]
    public Button closeBtn;
    [SerializeField] private Toggle toggleNarrator;
    [SerializeField] private Button leftLanguageBtn;
    [SerializeField] private Button rightLanguageBtn;
    [Space]
    [Header("Slider")]
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider SFXSlider;
    [SerializeField] private Slider narratorSlider;
    [Space]
    [Header("Text Display")]
    [SerializeField] private TextMeshProUGUI closeBtnText;
    [SerializeField] private TextMeshProUGUI volumeTitle;
    [SerializeField] private TextMeshProUGUI volumeNarratorTitle;
    [SerializeField] private TextMeshProUGUI musicVolumeText;
    [SerializeField] private TextMeshProUGUI SFXVolumeText;
    [SerializeField] private TextMeshProUGUI languageTitle;
    [SerializeField] private TextMeshProUGUI narratorTitle;
    [SerializeField] private TextMeshProUGUI textToSpeechText;
    [SerializeField] private TextMeshProUGUI offText;
    [SerializeField] private TextMeshProUGUI onText;
    [SerializeField] private TextMeshProUGUI narratorVolumeText;
    [Space]
    [Header("Text Value")]
    [SerializeField] private TextMeshProUGUI musicValueText;
    [SerializeField] private TextMeshProUGUI SFXValueText;
    [SerializeField] private TextMeshProUGUI languageValueText;
    [SerializeField] private TextMeshProUGUI narratorValueText;

    private readonly float onPosHandleX = 60f;
    private readonly float offPosHandleX = -60f;
    private Color onColorHandle;
    private Color offColorHandle;
    private Color onColorBGImageToggle;
    private Color offColorBGImageToggle;

    private void Start()
    {
        onColorHandle = GetColorFromHex("#fe0000");
        offColorHandle = GetColorFromHex("#505050");

        onColorBGImageToggle = GetColorFromHex("#3f4a78");
        offColorBGImageToggle = GetColorFromHex("#dadada");

        toggleNarrator.onValueChanged.AddListener(OnToggleNarratorChanged);

        musicSlider.onValueChanged.AddListener((value) => OnSliderValueChanged(VolumesType.musicVolume, musicValueText, value));
        SFXSlider.onValueChanged.AddListener((value) => OnSliderValueChanged(VolumesType.SFXVolume, SFXValueText, value));
        narratorSlider.onValueChanged.AddListener((value) => OnSliderValueChanged(VolumesType.narratorVolume, narratorValueText, value));

        leftLanguageBtn.onClick.AddListener(OnLeftLanguageBtnClicked);
        rightLanguageBtn.onClick.AddListener(OnRightLanguageBtnClicked);

        closeBtn.onClick.AddListener(() => OnCloseBtnClicked?.Invoke());

        GameManager.instance.settings.OnLangChanged += DisplayText;
        SettingsOn();
    }

    private void SettingsOn()
    {
        SetValueSlider(musicSlider, musicValueText, GameManager.instance.settings.musicVolume);
        SetValueSlider(SFXSlider, SFXValueText, GameManager.instance.settings.SFXVolume);
        SetValueSlider(narratorSlider, narratorValueText, GameManager.instance.settings.narratorVolume);

        SetToggleNarrator(toggleNarrator);
        OnLanguageChanged((int)GameManager.instance.settings.Language);
    }

    private void OnLeftLanguageBtnClicked()
    {
        int currentIndex = (int)GameManager.instance.settings.Language;
        if (currentIndex > 0)
        {
            currentIndex--;
            GameManager.instance.settings.Language = (Language)currentIndex;
            OnLanguageChanged(currentIndex);
        }
    }

    private void OnRightLanguageBtnClicked()
    {
        int currentIndex = (int)GameManager.instance.settings.Language;
        if (currentIndex < System.Enum.GetValues(typeof(Language)).Length - 1)
        {
            currentIndex++;
            GameManager.instance.settings.Language = (Language)currentIndex;
            OnLanguageChanged(currentIndex);
        }
    }

    private void DisplayText()
    {
        Language lang = GameManager.instance.settings.Language;
        var textData = GameManager.instance.localizedMainMenuText;

        volumeTitle.text = textData.settings.volume.GetText(lang);
        volumeNarratorTitle.text = textData.settings.volume.GetText(lang);
        musicVolumeText.text = textData.settings.musicVolume.GetText(lang);
        SFXVolumeText.text = textData.settings.SFXVolume.GetText(lang);

        languageTitle.text = textData.settings.language.GetText(lang);

        narratorTitle.text = textData.settings.narrator.GetText(lang);
        textToSpeechText.text = textData.settings.textToSpeech.GetText(lang);
        offText.text = textData.settings.off.GetText(lang);
        onText.text = textData.settings.on.GetText(lang);
        narratorVolumeText.text = textData.settings.volume.GetText(lang);

        closeBtnText.text = textData.closeBtn.GetText(lang);
    }

    #region Utility 
    private Color GetColorFromHex(string hex)
    {
        if(ColorUtility.TryParseHtmlString(hex, out Color color))
        {
            return color;
        }
        return Color.white;
    }

    private void SetValueSlider(Slider slider, TextMeshProUGUI text, float data)
    {
        slider.value = data % 100;
        int dataText = Mathf.RoundToInt(data * 100);
        text.text = dataText.ToString();
    }

    private void OnSliderValueChanged(VolumesType type, TextMeshProUGUI textDisplay, float value)
    {
        if (textDisplay != null)
        {
            int newValue = Mathf.RoundToInt(value * 100);
            textDisplay.text = newValue.ToString();
        }

        switch (type)
        {
            case VolumesType.musicVolume:
                GameManager.instance.settings.musicVolume = value;
                break;
            case VolumesType.SFXVolume:
                GameManager.instance.settings.SFXVolume = value;
                break;
            case VolumesType.narratorVolume:
                GameManager.instance.settings.narratorVolume = value;
                break;

        }

        GameManager.instance.SaveData<Settings>("settings", GameManager.instance.settings);
    }

    private void SetToggleNarrator(Toggle narrator)
    {
        bool isOn = GameManager.instance.settings.narrator;
        narrator.SetIsOnWithoutNotify(isOn);

        float targetX = isOn ? onPosHandleX : offPosHandleX;
        Color targetColor = isOn ? onColorHandle : offColorHandle;
        Color targetBGColor = isOn ? onColorBGImageToggle : offColorBGImageToggle;

        Vector2 newPos = toggleHandle.anchoredPosition;
        newPos.x = targetX;

        toggleHandle.anchoredPosition = newPos;
        toggleHandle.GetComponent<Image>().color = targetColor;
        bgImageToggle.color = targetBGColor;
    }

    private void OnToggleNarratorChanged(bool isOn)
    {
        // Toggle Handle
        float targetX = isOn ? onPosHandleX : offPosHandleX;
        Color targetColor = isOn ? onColorHandle : offColorHandle;
        Image image = toggleHandle.GetComponent<Image>();

        Vector2 newPos = toggleHandle.anchoredPosition;
        newPos.x = targetX;

        Tween.UIAnchoredPosition(toggleHandle, endValue: newPos, duration: 0.5f, ease: Ease.OutBack);
        Tween.Color(image, endValue: targetColor, duration: 0.5f, ease: Ease.InOutBack);

        // Toggle Background
        Color targetBGColor = isOn ? onColorBGImageToggle : offColorBGImageToggle;
        Tween.Color(bgImageToggle, endValue: targetBGColor, duration: 0.5f, ease: Ease.InOutQuad);

        GameManager.instance.settings.narrator = isOn;
        GameManager.instance.SaveData<Settings>("settings", GameManager.instance.settings);
    }

    private void OnLanguageChanged(int index)
    {
        languageFlags.sprite = GameManager.instance.flags[index];
        languageValueText.text = GameManager.instance.settings.Language == Language.en ? "English" : "Indonesia";
        GameManager.instance.SaveData<Settings>("settings", GameManager.instance.settings);
    }
    #endregion

}
