using System;
using System.Linq;
using PrimeTween;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[Serializable]
public struct QuizzActive
{
    public int number;
    public QuizzData quizzData;
}

public class QuizzManager : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private RectTransform leftNumber;
    [SerializeField] private RectTransform activeNumber;
    [SerializeField] private RectTransform rightNumber;
    [SerializeField] private TextMeshProUGUI question;
    [SerializeField] private Button[] options;
    public Button quizzBtn;
    [SerializeField] private GameObject quizzPanel;
    [SerializeField] private Button quizzNextBtn;
    [SerializeField] private Button quizzPreviousBtn;
    [SerializeField] private TextMeshProUGUI msg;
    [SerializeField] private TextMeshProUGUI quizzTimeText;
    [SerializeField] private Button submitBtn;

    [Space]

    [Header("Sprite Options")]
    [SerializeField] private Sprite defaultOption;
    [SerializeField] private Sprite selectedOption;
    [SerializeField] private Sprite correctOption;
    [SerializeField] private Sprite incorrectOption;

    [Space]

    [Header("Settings")]
    public bool QuizzStarted;
    [SerializeField] private float durationQuizzTime;

    [Space]

    public QuizzModelData quizzModelData;
    [SerializeField] private QuizzModel currentQuizz;
    [SerializeField] private SpawnManager spawnManager;
    private int optionButtonClicked;
    private int currentNumber;
    [SerializeField] private WorldManager manager;
    private PrimeTween.Sequence sequence;
    private PrimeTween.Sequence animSubmitBtn;

    private void OnEnable() => ImageReader.OnImage += QuizzReady;
    private void OnDisable() => ImageReader.OnImage -= QuizzReady;

    private void Start()
    {
        quizzNextBtn.onClick.AddListener(OnNextButtonClicked);
        quizzPreviousBtn.onClick.AddListener(OnPreviousButtonClicked);
        submitBtn.onClick.AddListener(OnSubmitBtnClicked);
        quizzBtn.onClick.AddListener(OnQuizzButtonClicked);

        quizzBtn.gameObject.SetActive(false);
    }

    #region Animated
    private void AnimatedNumbers()
    {
        CanvasGroup leftNumberCG = leftNumber.GetComponent<CanvasGroup>();
        CanvasGroup rightNumberCG = rightNumber.GetComponent<CanvasGroup>();

        TextMeshProUGUI leftNumberText = leftNumber.GetComponentInChildren<TextMeshProUGUI>();
        TextMeshProUGUI rightNumberText = rightNumber.GetComponentInChildren<TextMeshProUGUI>();
        TextMeshProUGUI activeNumberText = activeNumber.GetComponentInChildren<TextMeshProUGUI>();

        bool isFirstNumber = currentNumber == 1;
        bool isLastNumber = currentNumber == quizzModelData.quizzModel.Length;

        leftNumberCG.alpha = isFirstNumber ? 0f : 0.5f;
        rightNumberCG.alpha = isLastNumber ? 0f : 0.5f;

        leftNumberText.alpha = isFirstNumber ? 0f : 0.5f;
        rightNumberText.alpha = isLastNumber ? 0f : 0.5f;

        leftNumberText.text = (currentNumber - 1).ToString();
        rightNumberText.text = (currentNumber + 1).ToString();
        activeNumberText.text = currentNumber.ToString();
    }

    public void AnimatedQuizzTime()
    {
        QuizzStarted = true;

        bool langEN = GameManager.instance.settings.Language == Language.en;
        LocalizedMainMenuText localizedText = GameManager.instance.localizedMainMenuText;
        RectTransform quizzTimeRect = quizzTimeText.GetComponent<RectTransform>();

        quizzTimeText.text = langEN ? localizedText.quizzTime.en : localizedText.quizzTime.id;
        quizzTimeRect.pivot = new(0.5f, 0.5f);
        quizzTimeRect.localScale = Vector3.zero;
        quizzTimeText.alpha = 0f;

        quizzTimeText.gameObject.SetActive(true);

        if (sequence.isAlive) sequence.Stop();
        SoundManager.instance.StopBGM();

        sequence = Sequence.Create();

        sequence
        .ChainCallback(target: SoundManager.instance, target => target.PlaySFXSound(target.quizzTimeSound))
        .Group(Tween.Scale(quizzTimeRect, endValue: Vector3.one, duration: durationQuizzTime, ease: Ease.OutBack))
        .Group(Tween.Alpha(quizzTimeText, endValue: 1f, duration: durationQuizzTime / 2, ease: Ease.OutSine))
        .ChainDelay(0.5f)
        .Chain(Tween.Scale(quizzTimeRect, endValue: Vector3.zero, duration: durationQuizzTime, ease: Ease.InSine))
        .Group(Tween.Alpha(quizzTimeText, endValue: 0f, duration: durationQuizzTime / 2, ease: Ease.InSine))
        .OnComplete(target: this, target =>
        {
            target.quizzTimeText.gameObject.SetActive(false);
            target.currentNumber = 1;
            target.AnimatedQuizzPanel(true);
            target.QuizzStart();
            target.AnimatedSubmitBtn(false, 0.5f, true);
        });
    }

    public void AnimatedQuizzPanel(bool isOpen)
    {
        CanvasGroup cg = quizzPanel.GetComponent<CanvasGroup>();
        RectTransform rect = quizzPanel.GetComponent<RectTransform>();

        Vector3 startScale = isOpen ? Vector3.zero : Vector3.one;
        Vector3 endScale = isOpen ? Vector3.one : Vector3.zero;

        float startAlpha = isOpen ? 0f : 1f;
        float endAlpha = isOpen ? 1f : 0f;

        rect.pivot = new(0.5f, 0.5f);

        quizzPanel.SetActive(true);
        submitBtn.gameObject.SetActive(false);

        Sequence tween = Sequence.Create();

        if (isOpen)
        {
            tween.ChainCallback(target: SoundManager.instance, target => target.ChangeBGM(target.bgmQuizz));
        }
        tween
        .Group(Tween.Scale(rect, startValue: startScale, endValue: endScale, duration: durationQuizzTime, ease: Ease.OutSine))
        .Group(Tween.Alpha(cg, startValue: startAlpha, endValue: endAlpha, duration: durationQuizzTime, ease: Ease.OutSine))
        .OnComplete(target: this, target =>
        {
            target.submitBtn.gameObject.SetActive(true);
            if (!isOpen)
            {
                SoundManager.instance.ChangeBGM(SoundManager.instance.bgmGameplay);
            }
        });


    }

    public void AnimatedSubmitBtn(bool isEndNumber, float duration, bool isInstant)
    {
        RectTransform rect = submitBtn.GetComponent<RectTransform>();
        CanvasGroup cg = submitBtn.GetComponent<CanvasGroup>();
        TextMeshProUGUI tmp = submitBtn.GetComponentInChildren<TextMeshProUGUI>();

        float startPosX = isEndNumber ? 800f : 0f;
        float targetPosX = isEndNumber ? 0f : 800f;

        float startAlpha = isEndNumber ? 0f : 1f;
        float targetAlpha = isEndNumber ? 1f : 0f;

        Ease targetEase = isEndNumber ? Ease.OutSine : Ease.InSine;

        if (animSubmitBtn.isAlive) animSubmitBtn.Stop();

        if (!isInstant)
        {
            animSubmitBtn = Sequence.Create();

            animSubmitBtn
            .Group(Tween.Alpha(cg, startValue: startAlpha, endValue: targetAlpha, duration: duration, ease: targetEase))
            .Group(Tween.Alpha(tmp, startValue: startAlpha, endValue: targetAlpha, duration: duration, ease: targetEase))
            .Group(Tween.UIAnchoredPositionX(rect, startValue: startPosX, endValue: targetPosX, duration: duration, ease: targetEase));
        }
        else
        {
            submitBtn.gameObject.SetActive(false);
            cg.alpha = targetAlpha;
            tmp.alpha = targetAlpha;
            rect.anchoredPosition = new(targetPosX, rect.anchoredPosition.y);
        }
    }

    private void AnimatedDisableQuizzBtn(bool disable)
    {
        Image image = quizzBtn.GetComponent<Image>();

        Ease ease = disable ? Ease.InBack : Ease.OutBack;
        Color startColor = disable ? quizzBtn.colors.normalColor : quizzBtn.colors.disabledColor;
        Color endColor = disable ? quizzBtn.colors.disabledColor : quizzBtn.colors.normalColor;

        Tween.Custom(target: image, startValue: startColor, endValue: endColor, duration: 0.5f, ease: ease, onValueChange: (img, updatedColor) =>
        {
            img.color = updatedColor;
        });
    }
    #endregion

    public void QuizzReady(string text)
    {
        if (GameManager.instance == null) return;

        QuizzModelData newData = GameManager.instance.quizzModelData.FirstOrDefault(data => data.name == text);
        if (newData == null) return;

        if (quizzModelData != newData)
        {
            quizzModelData = newData;
            Debug.Log("quizz dipanggil terisi");
        }
    }

    public void DisableQuizzBtnWhenDialogRun(bool disable)
    {
        if (!quizzBtn.gameObject.activeSelf) return;

        quizzBtn.interactable = !disable;
        AnimatedDisableQuizzBtn(disable);
    }

    private void OnSubmitBtnClicked()
    {
        SoundManager.instance.PlaySFXSound(SoundManager.instance.clickSound);
        AnimatedQuizzPanel(false);
        QuizzStarted = false;
        quizzModelData.alreadyCompleted = true;
        quizzBtn.gameObject.SetActive(true);
    }

    private void OnQuizzButtonClicked()
    {
        SoundManager.instance.PlaySFXSound(SoundManager.instance.clickSound);
        quizzTimeText.gameObject.SetActive(false);
        currentNumber = 1;
        AnimatedQuizzPanel(true);
        QuizzStart();
        AnimatedSubmitBtn(false, 0.5f, true);
    }

    private void OnNextButtonClicked()
    {
        if (currentNumber < quizzModelData.quizzModel.Length)
        {
            SoundManager.instance.PlaySFXSound(SoundManager.instance.clickSound);
            if (currentQuizz.isClicked[currentQuizz.correntAnswerIndex])
            {
                currentNumber++;
                QuizzStart();

                if (currentNumber == quizzModelData.quizzModel.Length && currentQuizz.isClicked[currentQuizz.correntAnswerIndex])
                {
                    AnimatedSubmitBtn(true, 0.5f, false);
                }

                return;
            }

        }

        CheckAnswer();
        ChangeToDefault();
    }

    private void OnPreviousButtonClicked()
    {
        if (currentNumber > 1)
        {
            SoundManager.instance.PlaySFXSound(SoundManager.instance.clickSound);
            if (currentNumber == quizzModelData.quizzModel.Length && currentQuizz.isClicked[currentQuizz.correntAnswerIndex])
            {
                AnimatedSubmitBtn(false, 0.5f, false);
            }
            currentNumber--;
            QuizzStart();
        }
    }

    public void OnOptionButtonClicked()
    {
        quizzNextBtn.interactable = true;
        msg.gameObject.SetActive(false);

        ChangeToDefault();

        SoundManager.instance.PlaySFXSound(SoundManager.instance.clickSound);

        GameObject btnObj = EventSystem.current.currentSelectedGameObject;
        ChangeOptions(btnObj.transform, selectedOption);

        optionButtonClicked = Array.FindIndex(options, data => data.gameObject == btnObj);
    }

    private void CheckAnswer()
    {
        if (optionButtonClicked >= 0 && optionButtonClicked < currentQuizz.isClicked.Length)
        {
            if (optionButtonClicked == currentQuizz.correntAnswerIndex)
            {
                SoundManager.instance.PlaySFXSound(SoundManager.instance.correctSound);
                for (int i = 0; i < currentQuizz.isClicked.Length; i++)
                {
                    currentQuizz.isClicked[i] = true;
                    options[i].interactable = false;
                }

                if (GameManager.instance.settings.Language == Language.en)
                {
                    msg.text = GameManager.instance.localizedMainMenuText.messageQuizz.correct.en;
                }
                else
                {
                    msg.text = GameManager.instance.localizedMainMenuText.messageQuizz.correct.id;
                }

                if (currentNumber == quizzModelData.quizzModel.Length)
                {
                    AnimatedSubmitBtn(true, 0.5f, false);
                }

                if (currentNumber == quizzModelData.quizzModel.Length)
                {
                    quizzNextBtn.interactable = false;
                }
            }
            else
            {
                SoundManager.instance.PlaySFXSound(SoundManager.instance.incorrectSound);
                currentQuizz.isClicked[optionButtonClicked] = true;
                options[optionButtonClicked].interactable = false;
                quizzNextBtn.interactable = false;

                if (GameManager.instance.settings.Language == Language.en)
                {
                    msg.text = GameManager.instance.localizedMainMenuText.messageQuizz.incorrect.en;
                }
                else
                {
                    msg.text = GameManager.instance.localizedMainMenuText.messageQuizz.incorrect.id;
                }
            }
            msg.gameObject.SetActive(true);
        }
    }

    private void ChangeToDefault()
    {
        for (int i = 0; i < currentQuizz.isClicked.Length; i++)
        {
            if (!currentQuizz.isClicked[i])
            {
                ChangeOptions(options[i].transform, defaultOption);
            }
            else
            {
                if (i == currentQuizz.correntAnswerIndex)
                {
                    ChangeOptions(options[i].transform, correctOption);
                }
                else
                {
                    ChangeOptions(options[i].transform, incorrectOption);
                }
            }
        }
    }

    private void ChangeOptions(Transform transform, Sprite sprite)
    {
        foreach (Transform child in transform)
        {
            Image image = child.GetComponent<Image>();
            if (image != null)
            {
                image.sprite = sprite;
                return;
            }
        }

    }
    private void QuizzStart()
    {
        if (quizzModelData != null)
        {
            currentQuizz = quizzModelData.quizzModel.FirstOrDefault(data => data.number == currentNumber);
        }

        if (currentNumber > 1)
        {
            quizzPreviousBtn.interactable = true;
        }
        else
        {
            quizzPreviousBtn.interactable = false;
        }

        optionButtonClicked = 99;

        if (currentNumber < quizzModelData.quizzModel.Length)
        {
            quizzNextBtn.interactable = currentQuizz.isClicked[currentQuizz.correntAnswerIndex];
        }
        else
        {
            quizzNextBtn.interactable = false;
        }

        for (int i = 0; i < options.Length; i++)
        {
            options[i].interactable = !currentQuizz.isClicked[i];
        }

        ChangeToDefault();
        AnimatedNumbers();
        msg.gameObject.SetActive(false);

        if (GameManager.instance.settings.Language == Language.en)
        {
            question.text = currentQuizz.question.en;
            for (int i = 0; i < options.Length; i++)
            {
                options[i].GetComponentInChildren<TextMeshProUGUI>().text = currentQuizz.options.en[i];
            }
            submitBtn.GetComponentInChildren<TextMeshProUGUI>().text = GameManager.instance.localizedMainMenuText.submitBtn.en;
        }
        else if (GameManager.instance.settings.Language == Language.id)
        {
            question.text = currentQuizz.question.id;
            for (int i = 0; i < options.Length; i++)
            {
                options[i].GetComponentInChildren<TextMeshProUGUI>().text = currentQuizz.options.id[i];
            }
            submitBtn.GetComponentInChildren<TextMeshProUGUI>().text = GameManager.instance.localizedMainMenuText.submitBtn.id;
        }
    }
}
