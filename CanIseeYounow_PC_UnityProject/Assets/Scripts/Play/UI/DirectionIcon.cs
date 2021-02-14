using UnityEngine;

public class DirectionIcon : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            float X_Rotation = Input.GetAxis("Mouse X") * 4f;
            transform.Rotate(0, 0, X_Rotation);
        }
    }
}
