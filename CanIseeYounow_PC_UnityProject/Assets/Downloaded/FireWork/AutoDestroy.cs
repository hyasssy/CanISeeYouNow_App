using UnityEngine;
using Cysharp.Threading.Tasks;
using System;

public class AutoDestroy : MonoBehaviour
{
    [SerializeField]
    float _lifeTime = 7f;
    private void Start()
    {
        Kill().Forget();
    }
    async UniTask Kill()
    {
        await UniTask.Delay(TimeSpan.FromSeconds((double)_lifeTime), false, PlayerLoopTiming.Update);
        Destroy(gameObject);
    }
}
