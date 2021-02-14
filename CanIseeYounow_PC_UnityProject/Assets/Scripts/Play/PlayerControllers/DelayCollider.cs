using UnityEngine;
using Cysharp.Threading.Tasks;

public class DelayCollider : MonoBehaviour
{
    [SerializeField]
    Collider[] _colliders;
    async void Start()
    {
        await UniTask.Delay(3000);//かなり雑だが、最初コライダー切っておく。
        foreach(Collider c in _colliders)
        c.enabled = true;
    }
}
