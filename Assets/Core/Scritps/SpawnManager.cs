using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    #region Scripts
    [Header("Scripts")]
    [SerializeField] private WorldManager manager;
    #endregion
    [SerializeField] private List<QRGrouping> objectSpawned;

    #region Check Distance
    [Header("Check Distance Configuration")]
    [SerializeField] private float checkInterval = 0.5f;
    [SerializeField] private float maxDistance = 15.0f;
    private Coroutine distanceCheck;
    #endregion

    private void OnEnable()
    {
        Mapping.OnQRMapping += ObjectSpawn;
    }

    private void OnDisable()
    {
        Mapping.OnQRMapping -= ObjectSpawn;
    }

    private void ObjectSpawn(string text, bool isSpawning, Pose pose)
    {
        if (!isSpawning) return;

        if (System.Enum.TryParse(text, out QRTextList textEnum))
        {
            if (objectSpawned.Exists(item => item.QRText == textEnum))
            {
                int foundedIndex = objectSpawned.FindIndex(item => item.QRText == textEnum);
                QRGrouping foundedObject = objectSpawned[foundedIndex];
                foundedObject.parentObject.SetActive(true);
                StartDistanceCheck();
                return;
            }

            int foundIndex = manager.QRObject.FindIndex(item => item.QRText == textEnum);
            if (foundIndex != -1)
            {
                QRGrouping foundObject = manager.QRObject[foundIndex];
                if (foundObject.parentObject == null) return;

                GameObject newObject = Instantiate(foundObject.parentObject, pose.position, Quaternion.identity);

                QRGrouping newQRGrouping = new()
                {
                    QRText = textEnum,
                    parentObject = newObject
                };

                objectSpawned.Add(newQRGrouping);
                StartDistanceCheck();
                Debug.Log(foundObject.QRText);
            }
            else
            {
                Debug.Log("Data tidak ditemukan");
            }
        }
        else
        {
            Debug.Log("Tidak ada data tersebut di enum");
        }
    }

    private void StartDistanceCheck()
    {
        distanceCheck ??= StartCoroutine(DistanceCheckRoutine());
    }

    private void StopDistanceCheck()
    {
        if (distanceCheck != null)
        {
            StopCoroutine(DistanceCheckRoutine());
            manager.objectActive.point = QRTextList.empty;
            distanceCheck = null;
        }
    }

    private IEnumerator DistanceCheckRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(checkInterval);

            int objectIndex = objectSpawned.FindIndex(item => item.QRText == manager.objectActive.point);

            if (objectIndex == -1)
            {
                StopDistanceCheck();
                yield break;
            }

            QRGrouping currentObject = objectSpawned[objectIndex];
            if (!currentObject.parentObject.activeSelf)
            {
                StopDistanceCheck();
                yield break;
            }
            else
            {
                Vector3 playerPosition = Camera.main.transform.position;
                float distance = Mathf.Abs(Vector3.Distance(playerPosition, currentObject.parentObject.transform.position));
                if (distance > maxDistance && !manager.objectActive.active)
                {
                    currentObject.parentObject.SetActive(false);
                    StopDistanceCheck();
                    yield break;
                }
            }

        }
    }
}
