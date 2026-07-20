using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARPlaneManager))]
[RequireComponent(typeof(ARRaycastManager))]
public class Mapping : MonoBehaviour
{
    public static event Action<string, bool, Pose> OnQRMapping;
    [Header("Scritps")]
    [SerializeField] private WorldManager manager;
    [SerializeField] private AnimatedNotification animatedNotification;
    [Header("AR Components")]
    [SerializeField] private ARPlaneManager planeManager;
    [SerializeField] private ARRaycastManager raycastManager;

    private void Start()
    {
        if (planeManager == null)
        {
            planeManager = GetComponent<ARPlaneManager>();
        }

        if (raycastManager == null)
        {
            raycastManager = GetComponent<ARRaycastManager>();
        }

        if (planeManager == null || raycastManager == null)
        {
            Debug.LogError("AR components is missing");
        }
    }

    private void OnEnable()
    {
        ImageReader.OnImage += BelowPlayerScanned;
    }

    private void OnDisable()
    {
        ImageReader.OnImage -= BelowPlayerScanned;
    }

    private void BelowPlayerScanned(string text)
    {
        Vector3 rayOrigin = Camera.main.transform.position;
        Vector3 rayDirection = Vector3.down;

        Ray ray = new(rayOrigin, rayDirection);
        List<ARRaycastHit> hits = new();

        Pose pose = new();

        if (raycastManager.Raycast(ray, hits, TrackableType.Planes | TrackableType.PlaneWithinInfinity | TrackableType.PlaneWithinPolygon))
        {
            pose = hits[0].pose;
            OnQRMapping?.Invoke(text, true, pose);
        }
        else
        {
            OnQRMapping?.Invoke(text, false, pose);
        }
    }
}
