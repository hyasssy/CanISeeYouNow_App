using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class ZoomCamTest : MonoBehaviour
{
    KeyCode _key = KeyCode.K;
    [SerializeField]
    Transform _target;
    [SerializeField]
    float _duration = 1.5f;//何秒でむき終わるか
    [SerializeField]
    float _delay = 3;

    private void Update()
    {
        if (Input.GetKeyDown(_key))
        {
            ZoomIn().Forget();
        }
    }
    async UniTask ZoomIn()
    {
        //ここでマウスの入力（移動、視点回転）をカットする
        Camera main = Camera.main;
        float p = 0;
        float fieldOfView = main.fieldOfView;
        float primaryFieldOfView = main.fieldOfView;
        float targetFieldOfView = 30f;
        float time = 0;
        Quaternion primaryRot = transform.rotation;
        Quaternion targetRot = Quaternion.LookRotation(_target.position - transform.position, Vector3.up);
        while (time < _duration)
        {
            transform.rotation = Quaternion.Lerp(primaryRot, targetRot, p);
            main.fieldOfView = Mathf.Lerp(primaryFieldOfView, targetFieldOfView, p);
            time += Time.deltaTime;
            p = Easing.QuadInOut(time, _duration, 0, 1);
            await UniTask.Yield();
        }
        await UniTask.Delay(TimeSpan.FromSeconds(_delay));
        main.fieldOfView = primaryFieldOfView;
        //ここでマウスの入力（移動、視点回転）を復活
    }
}
