using UnityEngine;
using System;
using System.IO;

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
public class MultiLangText
{
    public string en;
    public string id;

    public string GetText(Language lang)
    {
        return lang == Language.en ? en : id;
    }
}

[Serializable]
public class LocalizedSettingsText
{
    public MultiLangText volume;
    public MultiLangText musicVolume;
    public MultiLangText SFXVolume;
    public MultiLangText language;
    public string[] flags;
    public MultiLangText narrator;
    public MultiLangText textToSpeech;
    public MultiLangText on;
    public MultiLangText off;
}

[Serializable]
public class LocalizedMainMenuText
{
    public MultiLangText startBtn;
    public MultiLangText settingsBtn;
    public MultiLangText creditsBtn;
    public MultiLangText exitBtn;
    public MultiLangText closeBtn;
    public LocalizedSettingsText settings;
}

[Serializable]
public class SceneDialog
{
    public int no;
    public string[] en;
    public string[] id;
}

[Serializable]
public class DestinationData
{
    public string name;
    public string narrator;
    public SceneDialog[] scene;
}

[Serializable]
public class RootDestination
{
    public DestinationData[] destination;
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

[Serializable]
public class Settings
{
    [SerializeField] private Language language;

    public Action OnLangChanged;
    public Language Language
    {
        get
        {
            return language;
        }
        set
        {
            if(language != value)
            {
                language = value;
                OnLangChanged?.Invoke();
            }
        }
    }

    public bool narrator;
    public float musicVolume;
    public float SFXVolume;
    public float narratorVolume;
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public DestinationData[] destination;
    public LocalizedMainMenuText localizedMainMenuText;
    public Sprite[] flags;
    public Settings settings;
    public QuizzModelData[] quizzModelData;

    private void Start()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        quizzModelData = LoadJsonFile<RootQuizzData>("quizz").quizzModelData;
        localizedMainMenuText = LoadJsonFile<LocalizedMainMenuText>("mainMenu");
        destination = LoadJsonFile<RootDestination>("dialogue").destination;

        settings = InstantiateData<Settings>("settings");
    }

    public void SaveData<T>(string fileName, T objectData)
    {
        string pathFile = Path.Combine(Application.persistentDataPath, fileName + ".json");

        if(!File.Exists(pathFile)) return;

        string jsonText = JsonUtility.ToJson(objectData, true);
        File.WriteAllText(pathFile, jsonText);
    }

    private T LoadJsonFile<T>(string fileName)
    {
        if (fileName != null)
        {
            TextAsset jsonFile = Resources.Load<TextAsset>(fileName);

            if (jsonFile != null)
            {
                return JsonUtility.FromJson<T>(jsonFile.text);
            }
        }

        return default;
    }

    private T InstantiateData<T>(string fileName)
    {
        string pathFile = Path.Combine(Application.persistentDataPath, fileName + ".json");

        if (File.Exists(pathFile)) return LoadData<T>(fileName);

        TextAsset jsonFile = Resources.Load<TextAsset>(fileName);

        if (jsonFile != null)
        {
            File.WriteAllText(pathFile, jsonFile.text);
            return LoadData<T>(fileName);
        }

        return default;
    }

    private T LoadData<T>(string fileName)
    {
        string path = Path.Combine(Application.persistentDataPath, fileName + ".json");
        if (File.Exists(path))
        {
            string data = File.ReadAllText(path);
            return JsonUtility.FromJson<T>(data);
        }
        else
        {
            return LoadJsonFile<T>(fileName);
        }
    }
}
