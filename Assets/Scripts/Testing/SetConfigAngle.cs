using System;
using NaughtyAttributes;
using Unity.Mathematics;
using UnityEngine;

public class SetConfigAngle : MonoBehaviour
{
    public ConfigurableJoint joint;
    public float angle;
    public float spring = 1000;
    public bool releaseSpring;
    public float increment;
    [ShowNativeProperty]
    public float currentEulerAngle => Vector3.SignedAngle(transform.up, Vector3.up,transform.right);

    public ActiveJoint ActiveJoint;

    private void Awake()
    {
        ActiveJoint = GetComponent<ActiveJoint>();
    }

    // Update is called once per frame
    void Update()
    {
        angle += increment * Time.deltaTime;
        var xdrive = joint.angularXDrive;
        xdrive.positionSpring = spring;
        joint.angularXDrive = xdrive;
        joint.targetRotation = Quaternion.AngleAxis(angle, Vector3.right);

    }
    [Button]
    public void SetAngle()
    {
        if(!enabled)
            Activate();
        angle = currentEulerAngle;
    }
    [Button]
    public void Activate()
    {
        ActiveJoint.enabled = false;
        GetComponentInParent<ActiveJointController>().enabled = false;
    }
    [Button]
    void Lock()
    {
        joint.yMotion = joint.zMotion = joint.angularXMotion = ConfigurableJointMotion.Locked;
    }
    [Button]
    void Unlock()
    {
        joint.yMotion = joint.zMotion = joint.angularXMotion = ConfigurableJointMotion.Free;
    }
    [Button]
    public void Deactivate()
    {
        GetComponent<ActiveJoint>().enabled = true;
        GetComponentInParent<ActiveJointController>().enabled = true;
        enabled = false;
    }
}
