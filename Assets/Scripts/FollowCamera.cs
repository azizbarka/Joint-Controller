using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public Transform target1;

    public Transform target2;
    public Vector3 placement;
    public float speed;
    void LateUpdate()
    {

        var targetPosition = Vector3.Lerp(target1.position, target2.position, .5f);
        transform.LookAt(targetPosition);
        targetPosition += placement;
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime * speed);

    }
}
