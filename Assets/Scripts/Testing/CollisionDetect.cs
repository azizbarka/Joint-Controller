using System;
using UnityEngine;

public class CollisionDetect : MonoBehaviour
{
    private void OnCollisionEnter(Collision other)
    {
        Debug.Log("Collision enter: " + other.gameObject.name);
    }
}
