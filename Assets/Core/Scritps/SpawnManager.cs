using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] ARAnchorManager anchorManager;
    [SerializeField] private QuizzManager quizzManager;
    [SerializeField] private AnimatedNotification animatedNotification;
    #region Scripts
    [Header("Scripts")]
    [SerializeField] private WorldManager manager;
    #endregion
    public List<QRGrouping> objectSpawned;

    #region Check Distance
    [Header("Check Distance Configuration")]
    [SerializeField] private float checkInterval = 0.5f;
    [SerializeField] private float maxDistance = 15.0f;
    private Coroutine distanceCheck;
    #endregion

    private void OnEnable()
    {
        Mapping.OnQRMapping += ObjectSpawn;
        manager.ObjectActive.PointChanged += AutoHideObject;
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

                if (foundedObject.parentObject.activeSelf) return;

                foundedObject.parentObject.SetActive(true);
                StartDistanceCheck();
                animatedNotification.Animated();
                return;
            }

            int foundIndex = manager.QRObject.FindIndex(item => item.QRText == textEnum);
            if (foundIndex != -1)
            {

                QRGrouping foundObject = manager.QRObject[foundIndex];
                if (foundObject.parentObject == null) return;

                GameObject anchor = new($"anchor_{text}");
                anchor.transform.position = pose.position;
                anchor.AddComponent<ARAnchor>();

                animatedNotification.Animated();
                quizzManager.quizzBtn.gameObject.SetActive(false);
                GameObject newObject = Instantiate(foundObject.parentObject, pose.position, Quaternion.identity);
                newObject.transform.SetParent(anchor.transform);

                QRGrouping newQRGrouping = new()
                {
                    QRText = textEnum,
                    parentObject = newObject
                };

                objectSpawned.Add(newQRGrouping);
                StartDistanceCheck();
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

    private void AutoHideObject(QRTextList oldObject)
    {
        int index = objectSpawned.FindIndex(item => item.QRText == oldObject);

        if (index != -1)
        {
            QRGrouping oldGrouping = objectSpawned[index];
            oldGrouping.parentObject.SetActive(false);
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
            manager.ObjectActive.Point = QRTextList.empty;
            distanceCheck = null;
        }
    }

    private IEnumerator DistanceCheckRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(checkInterval);

            int objectIndex = objectSpawned.FindIndex(item => item.QRText == manager.ObjectActive.Point);

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
                if (distance > maxDistance && !manager.ObjectActive.Active)
                {
                    currentObject.parentObject.SetActive(false);
                    StopDistanceCheck();
                    yield break;
                }
            }

        }
    }
}
