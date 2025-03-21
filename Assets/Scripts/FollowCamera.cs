using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public Transform target1;

    public Transform target2;
    public Vector3 placement;
    public float duration;
    private Vector3 smoothVelocity;
    void LateUpdate()
    {

        var targetPosition = Vector3.Lerp(target1.position, target2.position, .5f);
        targetPosition += placement;
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref smoothVelocity, duration);

    }
}
