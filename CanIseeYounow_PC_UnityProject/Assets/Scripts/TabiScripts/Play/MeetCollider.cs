using UnityEngine;
using UniRx;
using UniRx.Triggers;
using Cysharp.Threading.Tasks;

public class MeetCollider : MonoBehaviour
{
    [SerializeField]
    Collider _collider = default;
    void Start()
    {
        var sessionManager = FindObjectOfType<SessionManager>();
        _collider.OnTriggerEnterAsObservable()
            .Where(x => x.name == "CoreCollider" || x.name == "MeetCollider")
            .First()//1回目の値だけ通す
            .Subscribe(x =>
            {
                foreach (var c in FindObjectsOfType<PlayerController>())
                    c.ZoomIn().Forget();
                sessionManager.Meet().Forget();
            });
    }
}
