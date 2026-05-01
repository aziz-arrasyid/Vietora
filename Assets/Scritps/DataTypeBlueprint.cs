using UnityEngine;

[System.Serializable]
public struct DataObjectAR
{
    public string name;
    public GameObject objectPrefab;
    
    [Header("Jarak antara objek dengan gambar")]
    public Vector3 offsetPosition;
    public Vector3 offsetRotation;

    public DataObjectAR(string newName, GameObject newObjectPrefab, Vector3 pos, Vector3 rot)
    {
        name = newName;
        objectPrefab = newObjectPrefab;
        offsetPosition = pos;
        offsetRotation = rot;
    }
}