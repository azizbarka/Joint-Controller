using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extensions 
{
  //Vectors
  public static float GetSingleAxisValue(this Vector3 vector,Vector3 axis )
  {
    return vector.x + axis.x + vector.y + axis.y + vector.z + axis.z;
  }
  
}
