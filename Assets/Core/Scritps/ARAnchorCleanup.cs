using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ARAnchorCleanup : MonoBehaviour
{
    private void OnDestroy()
    {
        ARAnchor[] allAnchor = FindObjectsByType<ARAnchor>(FindObjectsSortMode.None);

        foreach (ARAnchor anchor in allAnchor)
        {
            if (anchor != null)
            {
                anchor.enabled = false;
            }
        }
    }
}
