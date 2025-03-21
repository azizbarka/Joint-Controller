using UnityEngine;

public class DisplayTestInfo : MonoBehaviour
{
    public ActiveJoint joint1;

    public TextMesh firstText;
    public ActiveJoint joint2;
    public TextMesh secondText;
    public Vector3 placement;
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        firstText.text = GetRigInfo(joint1);
        secondText.text = GetRigInfo(joint2);
        firstText.transform.position = joint1.transform.position + placement;
        secondText.transform.position = joint2.transform.position + placement;
    
    }
    private string GetRigInfo(ActiveJoint joint)
    {
        return $"Mass  : {joint.ActualRigidbody.mass}\nCollided :{joint.isCollided}\n" +
               $"Angles : ({joint.targetAngleRange.x} ,  {joint.targetAngleRange.y})\n" +
               $"Current : {joint.currentAngle}";
    }
}
