using System;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ImageScanner : MonoBehaviour
{
    private ARTrackedImageManager imageManager;
    [Header("Settings")]
    public static Action<string, Pose, bool> OnImageStatusChanged;
    [SerializeField] private bool imageScannerStatus = false;
    [SerializeField] private string imageScannerName = null;
    [SerializeField] private float maxDistance = 20f;
    private Pose imagePose;

    private void Awake()
    {
        imageManager = gameObject.GetComponent<ARTrackedImageManager>();
    }

    private void Update()
    {
        if (!imageScannerStatus) return;
        DisableScanner();
    }

    private void OnEnable() => imageManager.trackablesChanged.AddListener(OnChanged);
    private void OnDisable() => imageManager.trackablesChanged.RemoveListener(OnChanged);

    private void OnChanged(ARTrackablesChangedEventArgs<ARTrackedImage> eventArgs)
    {
        foreach (var trackedImage in eventArgs.updated)
        {
            imagePose = new Pose(trackedImage.transform.position, trackedImage.transform.rotation);

            if (trackedImage.trackingState == TrackingState.Tracking && DistanceCameraToImage(imagePose) <= maxDistance)
            {
                imageScannerStatus = true;
                imageScannerName = trackedImage.referenceImage.name;
                OnImageStatusChanged?.Invoke(imageScannerName, imagePose, imageScannerStatus);
            }
        }
    }

    private float DistanceCameraToImage(Pose imagePose)
    {
        return Vector3.Distance(Camera.main.transform.position, imagePose.position);
    }

    private void DisableScanner()
    {
        if (DistanceCameraToImage(imagePose) > maxDistance)
        {
            imageScannerStatus = false;
            imageScannerName = null;
            OnImageStatusChanged?.Invoke(imageScannerName, imagePose, imageScannerStatus);
        }
    }
}
