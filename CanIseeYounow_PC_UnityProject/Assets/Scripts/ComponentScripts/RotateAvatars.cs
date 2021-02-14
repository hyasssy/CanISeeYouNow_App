using UnityEngine;

public class RotateAvatars : MonoBehaviour
{
    [SerializeField]
    Transform[] _avatars;
    void Update()
    {
        foreach (Transform t in _avatars)
            t.localEulerAngles += Vector3.up * 30 * Time.deltaTime;
    }
}
