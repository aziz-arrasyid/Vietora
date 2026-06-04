using System;
using UnityEngine;
using UnityEngine.Events;

public class AreaZone : MonoBehaviour
{
    public static event Action<bool, GameObject> PlayerInside;

    private readonly string playerTag = "Player";

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            PlayerInside?.Invoke(true, gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            PlayerInside?.Invoke(false, gameObject);
        }
    }
}
