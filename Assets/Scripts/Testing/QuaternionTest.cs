using UnityEngine;
using UnityEngine.UI;

public class QuaternionTest : MonoBehaviour
{
    public Transform RedArrow;
    public Transform BlueArrow;
    public Transform YellowArrow;

    public bool multiplyRedBlue;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        YellowArrow.transform.rotation = multiplyRedBlue
            ? RedArrow.rotation * BlueArrow.rotation
            : BlueArrow.rotation * RedArrow.rotation;
    }
}
