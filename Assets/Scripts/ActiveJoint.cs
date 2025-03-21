using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using NaughtyAttributes;
using TMPro.EditorUtilities;
using Unity.Mathematics;
using UnityEngine;

[RequireComponent(typeof(ConfigurableJoint))]
public class ActiveJoint : MonoBehaviour
{
    
    //Paramaters
    [SerializeField,Required] private ActiveJointController controller;
    public float targetSpring = 2000f;
    public Vector2 targetAngleRange = new Vector2(20,170);
    [Header("Speed/ Duration")]
    public float rotateSpeed = 20;
    [Space(3)]
    public TransitionTimeUnit handleUnit  = TransitionTimeUnit.Speed;
    [SerializeField] private float handle=1000f;
    [Space(3)]
    public TransitionTimeUnit releaseUnit;
    [SerializeField ] private float release = 0.05f;
    
    
    //Properties
    public float HandleFactor => handleUnit == TransitionTimeUnit.Duration ? targetSpring / handle : handle;
    public float ReleaseFactor => releaseUnit == TransitionTimeUnit.Duration ? targetSpring / release : release;

    [SerializeField,ReadOnly]
    private bool _isHandled;
    public bool isHandled
    {
        get
        {
            return _isHandled;
        }
        set
        {
            if (value != _isHandled)
            {
                _isHandled = value;
                OnHandlingJointChanged();
            }
        }
    }
 
    [field: SerializeField]
    public bool isCollided { get; set; }

    private Quaternion initialJointRotation;
    [field: SerializeField] public float currentAngle{ get; set; }
    public ConfigurableJoint ActualJoint => joint;
    public Rigidbody ActualRigidbody => _rigidbody;
    //Private variables
    private ConfigurableJoint joint;
    private Rigidbody _rigidbody;
    
 

    private void Start()
    {
        joint = GetComponent<ConfigurableJoint>();
        _rigidbody = GetComponent<Rigidbody>();


    }

    [Button]
    public void HandleJoint()
    {
        isHandled = true;
    }
    public void SetIntialRotation() => initialJointRotation = transform.rotation;
    private void Update()
    {
 
        var angularSpringX = joint.angularXDrive;
        float currentSpring, springSpeed;
        if (isHandled)
        {
            var currentTargetAngle = controller.input > 0 ? targetAngleRange.y : targetAngleRange.x;
            currentSpring = targetSpring;
            currentAngle =  Mathf.MoveTowards(currentAngle, currentTargetAngle, rotateSpeed * controller.absoluteInput* Time.deltaTime);
           // currentAngle += rotateSpeed * controller.input * -1 *Time.deltaTime;
            springSpeed = HandleFactor;
        }
        else
        {
            currentSpring = 0;
            springSpeed = ReleaseFactor;
        }
            
        angularSpringX.positionSpring = Mathf.MoveTowards(angularSpringX.positionSpring, currentSpring,
            springSpeed * Time.deltaTime);

        joint.targetRotation = Quaternion.AngleAxis(currentAngle, joint.axis);
        joint.angularXDrive = angularSpringX;
        
     if(joint.axis != Vector3.right)
         Debug.LogWarning("Joint is changed "  + joint.axis);
    }

    
    //Methods
    private void OnHandlingJointChanged()
    {
        return;
        if (isHandled)
        {
            var isRight = controller.rightJoint == this;
            currentAngle = Vector3.SignedAngle(transform.forward, Vector3.forward, Vector3.up);
            if (isRight)
            {
                targetAngleRange.x = currentAngle;
                targetAngleRange.y = currentAngle + 170;
            }
            else
            {
                targetAngleRange.x = currentAngle - 170;
                targetAngleRange.y = currentAngle;
            }
        }
    }


 

    public void SetController(ActiveJointController controller) => this.controller = controller;
    private void OnCollisionEnter(Collision other) => OnCollisionChanged(true);
    private void OnCollisionExit(Collision other) => OnCollisionChanged(false);
    private void OnCollisionChanged(bool isCollided)
    {
        this.isCollided = isCollided;
        controller.OnAciveJointCollisionChanged(this);
    }

}