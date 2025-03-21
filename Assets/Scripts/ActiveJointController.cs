using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;

public class ActiveJointController : MonoBehaviour
{
    [Header("Active Control Joints")]
    public ActiveJoint tailJoint;
    public ActiveJoint headJoint;
    [Header("Mass")]
    [SerializeField] private float handledMass = 20;
    [SerializeField] private float releasedMass = 1;
    [SerializeField] private float jointMass = 15;
    [SerializeField] private float massSmoothDuration = 0.2f;
    [SerializeField] private float jointMassDuration= 0.4f;

    
    [Space(20)]
    public List<ConfigurableJoint> joints;
    
    //Properties
    public ActiveJoint[] controlJoints => new[] { tailJoint, headJoint };

    [SerializeField]
    private ActiveJoint _handledJoint;
    public ActiveJoint handledJoint
    {
        get
        {
            return _handledJoint;
        }
        set
        {
            if (value != _handledJoint)
            {
                if (value != null)
                {
                    int startIndex, endIndex, shift;
                    if (value == tailJoint)
                    {
                        startIndex = 1;
                        endIndex = joints.Count ;
                        shift = -1;
                        joints[0].connectedBody = null;
                        SetJointState(tailJoint, true);
                        SetJointState(headJoint, false);
                        tailJoint.SetIntialRotation();
                    }
                    else 
                    {
                        startIndex = 0;
                        endIndex = joints.Count - 1;
                        shift = 1;
                        joints[joints.Count - 1].connectedBody = null;
                        SetJointState(headJoint, true);
                        SetJointState(tailJoint, false);
                        tailJoint.SetIntialRotation();
                    }

                    for (int i = startIndex; i < endIndex; i++)
                        joints[i].connectedBody = jointRigs[i + shift];
                }
                
                for (int i = 0; i < controlJoints.Length; i++)
                    controlJoints[i].isHandled = controlJoints[i] == value;
                _handledJoint = value;
            }
        }
        
    }

    [ShowNativeProperty]
    public ActiveJoint leftJoint =>
        tailJoint.transform.position.z < headJoint.transform.position.z ? tailJoint : headJoint;
    [ShowNativeProperty]
    public ActiveJoint rightJoint => GetOppositeJoint(leftJoint);
    public bool hasHandledJoint => handledJoint != null;
    public bool isControllInput => Input.GetMouseButton(0);
    [ShowNativeProperty]
    public float input => Input.mousePositionDelta.x;

    public float absoluteInput => Mathf.Abs(input);
    //Private variables
    private Rigidbody[] jointRigs;
    private float handleMassVelocity, releaseMassVelocity,handledJointMassVelocity,releasedJointMassVelocity;


    private void Start()
    {
        ControlTailJoint();
        jointRigs = joints.Select(j => j.GetComponent<Rigidbody>()).ToArray();
    }

    public void OnAciveJointCollisionChanged(ActiveJoint joint)
    {
        return;
        //Case joint fall to the ground
        if(handledJoint  == joint && joint.isCollided)
            ReleaseJoint();
        
        //Case opposite joint left from the ground
        if (GetOppositeJoint(joint))
        {
            
        }

            
    }

    private void SetJointState(ActiveJoint activeJoint, bool isHandled)
    {
        var joint = activeJoint.ActualJoint;
        if (isHandled)
        {
            joint.yMotion = joint.zMotion = joint.angularXMotion = ConfigurableJointMotion.Free;
        }
        else
            joint.yMotion = joint.zMotion = joint.angularXMotion = ConfigurableJointMotion.Locked;
        
    }

    public ActiveJoint GetOppositeJoint(ActiveJoint joint)
    {
        return joint == tailJoint ? headJoint : tailJoint;
    }


    [Button]
    public void MoveJoint()
    {
        var rig = rightJoint.ActualRigidbody;
        rig.AddForce(Vector3.up * 100 , ForceMode.VelocityChange);
    }
    private void ReleaseJoint() => handledJoint = null;



    private void Update()
    {
        if (hasHandledJoint)
        {
            if(!isControllInput)
                ReleaseJoint();
            else
            {
                var releasedJoint = GetOppositeJoint(handledJoint);
                var handledRig =handledJoint.ActualRigidbody;
                var releasedRig = releasedJoint.ActualRigidbody;
                //Rigidbody Mass
                handledRig.mass = Mathf.SmoothDamp(handledRig.mass, handledMass, ref handleMassVelocity,
                    massSmoothDuration);
                releasedRig.mass = Mathf.SmoothDamp(releasedRig.mass, releasedMass, ref releaseMassVelocity,
                    massSmoothDuration);
                //Joint mass
                handledJoint.ActualJoint.massScale = Mathf.SmoothDamp(handledJoint.ActualJoint.massScale ,jointMass, ref handledJointMassVelocity,
                    jointMassDuration);
                releasedJoint.ActualJoint.massScale = Mathf.SmoothDamp(releasedJoint.ActualJoint.massScale ,releasedMass, ref releaseMassVelocity,
                    jointMassDuration * 0.5f);
            }
        }
        if (isControllInput && input != 0 && !hasHandledJoint)
        {
            if (input > 0 && rightJoint.isCollided)
            {
                handledJoint = leftJoint;
                handledJoint = rightJoint;
            }
            else if (input < 0 && leftJoint.isCollided)
            {
                handledJoint = rightJoint;
                handledJoint = leftJoint;
            }
            else
            {
                return;
            }
            handledJoint.currentAngle = 0;
        }
    }


    [Header("Gizmo")] 
    public bool showCollisionIndicator;
    public bool showSelectedBone  = true;

    public float selectionBoxSize = 5f;
    private void OnDrawGizmos()
    {
        //Cube drawing around the selected joint
        if (handledJoint && showSelectedBone)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(handledJoint.transform.position, Vector3.one * selectionBoxSize);
        }

        if (showCollisionIndicator)
        {
            var radius = selectionBoxSize * 0.2f;
  
            _ShowCollisionIndicator(headJoint);
            _ShowCollisionIndicator(tailJoint);
            //Internal method
            void _ShowCollisionIndicator(ActiveJoint joint)
            {
                Gizmos.color = joint.isCollided ? Color.blue : Color.red;
                Gizmos.DrawSphere( joint.transform.position ,  radius); 
            }
            
  
            
        }

    }
    
    #region  Inspector Buttons + Info
    [Button]
    public void FillAllJoints()
    {
        var activeJoints = GetComponentsInChildren<ActiveJoint>();
        joints = GetComponentsInChildren<ConfigurableJoint>().ToList();
        tailJoint = activeJoints[0];
        headJoint = activeJoints[1];
        tailJoint.SetController(this);
        headJoint.SetController(this);
    }
    
    [Button]
    private void ControlTailJoint()
    {
        handledJoint = tailJoint;
    }
    
    [Button]
    private void ReleaseControl()
    {
        handledJoint = null;
    }
    
    #endregion
    
 
    
    
   

}
