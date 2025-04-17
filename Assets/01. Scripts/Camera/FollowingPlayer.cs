using UnityEngine;

public class FollowingPlayer : MonoBehaviour
{
    public Transform target;

    public float followingSpeed;

    private void FixedUpdate()
    {
        Vector3 pos = Vector2.Lerp(transform.position, target.position, Time.fixedDeltaTime * followingSpeed);
        pos.z = transform.position.z;

        transform.position = pos;
    }
}
