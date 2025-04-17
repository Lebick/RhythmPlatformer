using UnityEngine;

public class FollowingCamera : MonoBehaviour
{
    private Transform targetCamera;

    private void Start()
    {
        targetCamera = Camera.main.transform;
    }

    private void LateUpdate()
    {
        transform.position = targetCamera.position;
    }
}
