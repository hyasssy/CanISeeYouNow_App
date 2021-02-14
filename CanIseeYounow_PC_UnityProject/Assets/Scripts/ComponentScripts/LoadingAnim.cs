using UnityEngine;

public class LoadingAnim : MonoBehaviour {
    [SerializeField]
    float _rotateSpeed = -240f;
    void Update () {
        transform.localEulerAngles += Vector3.forward * Time.deltaTime * _rotateSpeed;
    }
}