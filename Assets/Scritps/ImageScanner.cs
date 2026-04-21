using System;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ImageScanner : MonoBehaviour
{
    private ARTrackedImageManager imageManager;
    [Header("Settings")]
    [SerializeField] private bool imageScannerStatus = false;
    [SerializeField] private string imageScannerName = null;
    [SerializeField] private float maxDistance = 20f;
    private Vector3 scannerPosition = Vector3.zero;

    private void Awake()
    {
        imageManager = GetComponent<ARTrackedImageManager>();
    }

    private void Update()
    {
        if(!imageScannerStatus) return;
        DisableScanner();
    }

    private void OnEnable() => imageManager.trackablesChanged.AddListener(OnChanged);
    private void OnDisable() => imageManager.trackablesChanged.RemoveListener(OnChanged);

    private void OnChanged(ARTrackablesChangedEventArgs<ARTrackedImage> eventArgs)
    {
        foreach(var trackedImage in eventArgs.updated)
        {
            scannerPosition = trackedImage.transform.position;
            float distance = DistanceScanner(Camera.main.transform.position, scannerPosition);
            
            if(trackedImage.trackingState == TrackingState.Tracking && distance <= maxDistance)
            {
                imageScannerStatus = true;
                imageScannerName = trackedImage.referenceImage.name;

                Debug.Log(imageScannerName + ": " + imageScannerStatus);
            }
        }
    }

    private float DistanceScanner(Vector3 playerPosition, Vector3 scannerPosition)
    {
        return Vector3.Distance(playerPosition, scannerPosition);
    }

    private void DisableScanner()
    {
        float distance = DistanceScanner(Camera.main.transform.position, scannerPosition);

        if(distance > maxDistance)
        {
            imageScannerStatus = false;
            imageScannerName = null;
        }
    }
}
