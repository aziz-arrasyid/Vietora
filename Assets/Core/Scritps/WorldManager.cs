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

public struct ObjectActive
{
    public QRTextList point;
    public bool active;
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
    [Header("UI Components")]
    public TextMeshProUGUI mappingStatus;
    public TextMeshProUGUI QRStatus;

    [Header("Data Object Configuration")]
    public List<QRGrouping> QRObject;
    public ObjectActive objectActive;
}
