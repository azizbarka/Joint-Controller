using System;
using UnityEngine;

public class GameInput : MonoBehaviour
{


    public event Action OnSwipReleased;

    public void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            
        }
    }
}
