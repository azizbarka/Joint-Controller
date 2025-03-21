using NaughtyAttributes;
using UnityEngine;

public class QuatTests : MonoBehaviour
{

    public ConfigurableJoint joint;
    public Rigidbody parent;
    public float targetAngle;

    private Quaternion startRotation;


    void Update()
    {



        joint.targetRotation = Quaternion.AngleAxis(targetAngle, Vector3.right);// * startRotation;
    }

    public bool isConncted =>joint &&  joint.connectedBody != null;
    public bool isDisconnceted => !isConncted;

    [Button, ShowIf("isDisconnceted") ]
    public void ConnectParent()
    {
        startRotation = joint.transform.rotation;
        joint.connectedBody = parent;
    }
    [Button , ShowIf("isConncted") ]
    public void DisconncetdParent()
    {
        startRotation = joint.transform.rotation;
        joint.connectedBody = null;
    }
    

    
    
}
