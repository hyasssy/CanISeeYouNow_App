using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;
using Photon.Pun;

public class Bell : MonoBehaviour
{
    [SerializeField]
    bool _isHost = default;
    GameObject _bellPrefab;
    void Start()
    {
        _bellPrefab = Resources.Load<GameObject>("BellEffect");
        var photonView = GetComponent<PhotonView>();

        //Mockは一旦無視。
        PhotonMatchingManager matchingManager = FindObjectOfType<PhotonMatchingManager>();
        if (_isHost == matchingManager.IsHost)//それぞれのプレイヤーのシーンでホストの方のオブジェから呼び出す。
        {
            GameObject.Find("GUI/Canvas/StandardUIGroup/Bell")
            .GetComponent<Button>()
            .OnPointerClickAsObservable()
                .Subscribe(_ =>
                {
                    OnBellRang();
                    photonView.RPC(nameof(OnBellRang), RpcTarget.Others);
                })
                .AddTo(this);
        }
    }

    [PunRPC]
    void OnBellRang()
    {
        var name = _isHost ? "Host" : "Guest";
        Debug.Log($"{name}の鐘を鳴らす");
        Instantiate(_bellPrefab, transform.position, Quaternion.identity);
    }
}
