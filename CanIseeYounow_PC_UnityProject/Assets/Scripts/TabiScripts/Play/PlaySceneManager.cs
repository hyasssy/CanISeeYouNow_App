using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;
using Photon.Pun;

public class PlaySceneManager : MonoBehaviour
{
    IMatchingManager _matchingManager;
    void Start()
    {
        if (_matchingManager == null)
            _matchingManager = FindObjectOfType<PhotonMatchingManager>();
        _matchingManager.OnAnotherPlayerLoadScene//もう1人もプレイシーンにきた時
            .Where(x => x == NewScene.p5_Play)//Endシーンに行っちゃわないように、保険かけてる。
            .Subscribe(async _ =>
            {
                await UniTask.Yield();
                var debugParameter = Resources.Load<DebugParameter>("DebugParameter");
                if(debugParameter.EventType == 0 || debugParameter.IsUseVoiceChat)
                    PhotonNetwork.Instantiate("PhotonVoice", Vector3.zero, Quaternion.identity);//waitシーンから移行する時に作り直してる。（PhotonVoiceがBackgroundシーンに紐づいていたため）
                var mainPlayerRoot = _matchingManager.InstantiatePlayerRoot().transform;
                var cameras = GameObject.Find("Cameras").transform;//カメラは元々シーン上にあり、それをしかるべき場所に配置している
                cameras.SetParent(mainPlayerRoot.GetChild(0), false);
            });
    }
}
