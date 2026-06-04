using UnityEngine;

public class Player : MonoBehaviour
{
    private void LateUpdate()
    {
        transform.position = Camera.main.transform.position;
        Vector3 playerRotation = Camera.main.transform.eulerAngles;
        transform.rotation = Quaternion.Euler(0, playerRotation.y, 0);
    }
}
