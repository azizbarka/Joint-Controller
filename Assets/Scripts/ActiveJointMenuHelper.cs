using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public static class ActiveJointMenuHelper
{


    [MenuItem("Active Joint Controller/Create")]
    public static void CreateActiveJointController()
    {
        var selectedObject = Selection.activeGameObject;
        var jointController = selectedObject.AddComponent<ActiveJointController>();
        List<ConfigurableJoint> joints = new List<ConfigurableJoint>();
        if (selectedObject.transform.Find("Armature") != null)
            selectedObject = selectedObject.transform.Find("Armature").gameObject;
        for (int i = 0; i < selectedObject.transform.childCount; i++)
        {
            var child = selectedObject.transform.GetChild(i);
            if (child.name.ToLower().StartsWith("bone"))
            {
                child.AddComponent<Rigidbody>();
                child.AddComponent<SphereCollider>();
                joints.Add(child.AddComponent<ConfigurableJoint>());
                if (i > 0)
                    joints[i].connectedBody = joints[i - 1].GetComponent<Rigidbody>();
                joints[i].xMotion = joints[i].yMotion = joints[i].zMotion = joints[i].angularYMotion =
                    joints[i].angularZMotion = joints[i].angularXMotion = ConfigurableJointMotion.Locked;
            }
        }
        joints[0].yMotion = joints[0].zMotion =joints[0].angularXMotion =ConfigurableJointMotion.Free;
        jointController.handledJoint =   joints[0].AddComponent<ActiveJoint>();
        joints[joints.Count-1].AddComponent<ActiveJoint>();
        jointController.FillAllJoints();
    }
}
