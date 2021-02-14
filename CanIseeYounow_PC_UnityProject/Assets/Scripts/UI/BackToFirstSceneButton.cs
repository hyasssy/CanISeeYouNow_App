using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;

public class BackToFirstSceneButton : MonoBehaviour
{
    void Start()
    {
        var matchingManager = FindObjectOfType<PhotonMatchingManager>();
        GetComponent<Button>().OnPointerClickAsObservable()
            .Subscribe(_ => matchingManager.LeaveRoom());
    }
}
