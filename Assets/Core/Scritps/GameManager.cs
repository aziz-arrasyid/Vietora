using UnityEngine;
using System;

[Serializable]
public enum Language
{
    en,
    id
}

[Serializable]
public class LocalizedOptions
{
    public string[] id;
    public string[] en;
}

[Serializable]
public class LocalizedQuestion
{
    public string id;
    public string en;
}

[Serializable]
public class QuizzModel
{
    public int number;
    public LocalizedQuestion question;
    public LocalizedOptions options;
    public int correntAnswerIndex;
    public bool[] isClicked = new bool[4];
}

[Serializable]
public class QuizzModelData
{
    public string name;
    public QuizzModel[] quizzModel;
}

[Serializable]
public class RootQuizzData
{
    public QuizzModelData[] quizzModelData;
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public Language currentLanguage;
    public QuizzModelData[] quizzModelData;

    private void Start()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        LoadJsonFile("quizz");
    }

    private void LoadJsonFile(string fileName)
    {
        if (fileName != null)
        {
            TextAsset jsonFile = Resources.Load<TextAsset>(fileName);

            if (jsonFile != null)
            {
                RootQuizzData rootQuizzData = JsonUtility.FromJson<RootQuizzData>(jsonFile.text);
                quizzModelData = rootQuizzData.quizzModelData;
            }
        }
    }
}
