using System;
using System.Linq;
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
    [SerializeField] private TextMeshProUGUI question;
    [SerializeField] private Button[] options;
    [SerializeField] private GameObject QuizzBtn;
    [SerializeField] private GameObject quizzPanel;
    [SerializeField] private Button quizzNextBtn;
    [SerializeField] private Button quizzPreviousBtn;

    [Space]

    [SerializeField] private QuizzModelData quizzModelData;
    [SerializeField] private QuizzModel currentQuizz;
    [SerializeField] private SpawnManager spawnManager;
    private int optionButtonClicked;
    private int currentNumber;
    [SerializeField] private WorldManager manager;

    private void OnEnable() => ImageReader.OnImage += QuizzReady;
    private void OnDisable() => ImageReader.OnImage -= QuizzReady;

    public void QuizzReady(string text)
    {
        if (GameManager.instance == null) return;
        quizzModelData = GameManager.instance.quizzModelData.FirstOrDefault(data => data.name == text);
    }

    public void OnQuizzButtonClicked()
    {
        currentNumber = 1;
        quizzPanel.SetActive(true);
        QuizzStart();
    }

    public void OnNextButtonClicked()
    {
        if (currentNumber < quizzModelData.quizzModel.Length)
        {
            if (currentQuizz.isClicked[currentQuizz.correntAnswerIndex])
            {
                currentNumber++;
                QuizzStart();
                return;
            }

            CheckAnswer();
            ChangeToDefault();
        }
    }

    public void OnPreviousButtonClicked()
    {
        if (currentNumber > 1)
        {
            currentNumber--;
            QuizzStart();
        }
    }

    public void OnOptionButtonClicked()
    {
        quizzNextBtn.interactable = true;

        ChangeToDefault();

        GameObject btnObj = EventSystem.current.currentSelectedGameObject;
        btnObj.GetComponentInChildren<Image>().color = Color.black;

        optionButtonClicked = Array.FindIndex(options, data => data.gameObject == btnObj);
    }

    private void CheckAnswer()
    {
        if (optionButtonClicked >= 0 && optionButtonClicked < currentQuizz.isClicked.Length)
        {
            if (optionButtonClicked == currentQuizz.correntAnswerIndex)
            {
                for (int i = 0; i < currentQuizz.isClicked.Length; i++)
                {
                    currentQuizz.isClicked[i] = true;
                    options[i].interactable = false;
                }
            }
            else
            {
                currentQuizz.isClicked[optionButtonClicked] = true;
                options[optionButtonClicked].interactable = false;
                quizzNextBtn.interactable = false;
            }
        }
    }

    private void ChangeToDefault()
    {
        for (int i = 0; i < currentQuizz.isClicked.Length; i++)
        {
            if (!currentQuizz.isClicked[i])
            {
                options[i].GetComponentInChildren<Image>().color = Color.white;
            }
            else
            {
                if (i == currentQuizz.correntAnswerIndex)
                {
                    options[i].GetComponentInChildren<Image>().color = Color.green;
                }
                else
                {
                    options[i].GetComponentInChildren<Image>().color = Color.red;
                }
            }
        }
    }

    private void QuizzStart()
    {
        if (quizzModelData != null)
        {
            currentQuizz = quizzModelData.quizzModel.FirstOrDefault(data => data.number == currentNumber);
        }

        optionButtonClicked = 99;

        quizzNextBtn.interactable = currentQuizz.isClicked[currentQuizz.correntAnswerIndex];

        for (int i = 0; i < options.Length; i++)
        {
            options[i].interactable = !currentQuizz.isClicked[i];
        }

        ChangeToDefault();

        if (GameManager.instance.currentLanguage == Language.en)
        {
            question.text = currentQuizz.question.en;
            for (int i = 0; i < options.Length; i++)
            {
                options[i].GetComponentInChildren<TextMeshProUGUI>().text = currentQuizz.options.en[i];
            }
        }
        else if (GameManager.instance.currentLanguage == Language.id)
        {
            question.text = currentQuizz.question.id;
            for (int i = 0; i < options.Length; i++)
            {
                options[i].GetComponentInChildren<TextMeshProUGUI>().text = currentQuizz.options.id[i];
            }
        }
    }
}
