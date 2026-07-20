using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum QRTextList
{
    empty,
    ship_vietora,
    refugee_barracks_vietora,
    car_vietora,
    youth_center_vietora
}

[System.Serializable]
public struct QRGrouping
{
    public QRTextList QRText;
    public GameObject parentObject;
    public bool isReadingCompletedAll;
}

[Serializable]
public class MyObjectActive
{
    public event Action<QRTextList> PointChanged;
    [SerializeField] private QRTextList point;
    public QRTextList Point
    {
        get => point;
        set
        {
            if (point != value)
            {
                PointChanged?.Invoke(point);
                point = value;
            }
        }
    }

    public bool Active;
}

[System.Serializable]
public struct QuizzData
{
    [TextArea]
    public string question;
    public int correctAnswer;
    public string[] options;
}

public class WorldManager : MonoBehaviour
{
    [Header("Data Object Configuration")]
    public List<QRGrouping> QRObject;
    public MyObjectActive ObjectActive;

    private void Start()
    {
        SoundManager.instance.ChangeBGM(SoundManager.instance.bgmGameplay);
    }
}
