using System;
using TMPro;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARTrackedImageManager))]
public class ImageReader : MonoBehaviour
{
    [Header("Scripts")]
    [SerializeField] private WorldManager manager;

    #region AR
    private ARTrackedImageManager trackedImageManager;
    #endregion

    public static event Action<string> OnImage;

    private void Awake()
    {
        trackedImageManager = GetComponent<ARTrackedImageManager>();
    }

    private void OnEnable() => trackedImageManager.trackablesChanged.AddListener(OnTrackablesChanged);

    private void OnDisable() => trackedImageManager.trackablesChanged.RemoveListener(OnTrackablesChanged);

    private void OnTrackablesChanged(ARTrackablesChangedEventArgs<ARTrackedImage> eventArgs)
    {
        foreach (var trackedImage in eventArgs.updated)
        {
            if(trackedImage.trackingState == TrackingState.Tracking)
            {
                OnImage?.Invoke(trackedImage.referenceImage.name);
                if(Enum.TryParse(trackedImage.referenceImage.name, out QRTextList newEnum))
                {
                    manager.ObjectActive.Point = newEnum;
                    manager.ObjectActive.Active = true;
                }
            }
            else
            {
                manager.ObjectActive.Active = false;
            }
        }
    }
}
