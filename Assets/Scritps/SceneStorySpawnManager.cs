using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class SceneStorySpawnManager : MonoBehaviour
{
    [Header("Data Settings")]
    [SerializeField] private List<DataObjectAR> dataAsset;
    public List<DataObjectAR> spawnedObject = new();
    private string lastSeenImage = null;

    private void OnEnable() => ImageScanner.OnImageStatusChanged += HandleRespone;
    private void OnDisable() => ImageScanner.OnImageStatusChanged -= HandleRespone;

    private void HandleRespone(string imageName, Pose imagePose, bool scanningStatus)
    {
        if (scanningStatus && lastSeenImage != imageName && spawnedObject.Count == 0)
        {
            if (CheckDataAsset(imageName)) SpawnObject(imageName, imagePose);
        }
        else if(lastSeenImage != imageName && spawnedObject.Count > 0)
        {
            DestroyObject();
        }
    }

    private void SpawnObject(string imageName, Pose imagePose)
    {
        foreach (var asset in dataAsset)
        {
            if (asset.name.Contains(imageName) && !spawnedObject.Exists(obj => obj.name == asset.name))
            {
                if(asset.objectPrefab != null)
                {
                    GameObject obj = Instantiate(asset.objectPrefab, imagePose.position, Quaternion.identity);
                    
                    obj.transform.localPosition = asset.offsetPosition != Vector3.zero ? asset.offsetPosition : imagePose.position;
                    obj.transform.localEulerAngles += asset.offsetRotation;

                    spawnedObject.Add(new DataObjectAR(asset.name, obj, asset.offsetPosition, asset.offsetRotation));
                }
            }
        }
        lastSeenImage = imageName;
    }

    private void DestroyObject()
    {
        foreach(var obj in spawnedObject)
        {
            if(obj.objectPrefab != null) Destroy(obj.objectPrefab);
        }
        spawnedObject.Clear();
        lastSeenImage = null;
    }

    private bool CheckDataAsset(string imageName) => dataAsset.Exists(data => data.name.Contains(imageName));
}