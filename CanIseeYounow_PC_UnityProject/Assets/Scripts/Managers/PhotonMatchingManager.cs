using System;
using UnityEngine;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UniRx;
using Cysharp.Threading.Tasks;

//値を飛ばすのはここだけ
public class PhotonMatchingManager : MonoBehaviourPunCallbacks, IMatchingManager
{
    const string GAME_VERSION = "Ver1.0";//形式的に入れている

    //ルームオプションのプロパティー
    static RoomOptions _roomOptions = new RoomOptions()
    {
        MaxPlayers = 2, //0だと人数制限なし
        IsOpen = true, //部屋に参加できるか
        IsVisible = true, //この部屋がロビーにリストされるか
    };

    [SerializeField]
    AudioSource _connectedAudio;//Disconnected, connectedの音
    [SerializeField]
    AudioSource _disconnectedAudio;
    [HideInInspector]
    public NewSceneLoader newSceneLoader;

    void Start()
    {
        PhotonNetwork.GameVersion = GAME_VERSION;
        PhotonNetwork.ConnectUsingSettings();//photonのデフォルト設定を読み込む

        var photonView = GetComponent<PhotonView>();
        newSceneLoader.OnLoad
            .Subscribe(x => photonView.RPC(nameof(OnLoadScene), RpcTarget.Others, x))//もう一方のシーンのOnLoadSceneに、読み込んだシーンの名前を送信する。
            .AddTo(this);
        OnAnotherPlayerLoadScene
            .Subscribe(x => Debug.Log($"AnotherPlayer Load {x}"))
            .AddTo(this);

        DontDestroyOnLoad(gameObject);
    }

    ReactiveProperty<NewScene> _onAnotherPlayerLoadScene = new ReactiveProperty<NewScene>();
    public IReadOnlyReactiveProperty<NewScene> OnAnotherPlayerLoadScene => _onAnotherPlayerLoadScene.AddTo(this);

    [PunRPC]//photonView.RPC(メソッドの名前(nameofで参照もできる), RpcTarget.Others, 任意の型のパラメータ);
    void OnLoadScene(NewScene scene)
    {
        _onAnotherPlayerLoadScene.Value = scene;
    }

    public bool IsConnected { get; private set; } = false;
    public override void OnConnectedToMaster()//ロビーにつながったときに呼ばれる
    {
        Debug.Log("Lobby Connect");
        IsConnected = true;
    }

    public void JoinOrCreateRoom(string roomName)//指定した部屋に入る。
    {
        Debug.Log($"Try Join Room {roomName}!!!!!!!!!!!!!!!!!!");
        if (IsConnected)
            PhotonNetwork.JoinOrCreateRoom(roomName, _roomOptions, TypedLobby.Default);
    }
    int roomNum = 4;
    int i = 2;
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.LogWarning("Rejected to join a room.");
        if (i < roomNum)
        {
            JoinOrCreateRoom("room" + i.ToString());
            i++;
        }
        else
        {
            Debug.LogWarning("これ以上アクセスできません。");
        }
    }

    public Room Room { get; private set; }
    public Player Player { get; private set; }
    public bool IsHost => Player?.IsMasterClient ?? false;


    Subject<Unit> _onJoin = new Subject<Unit>();
    //Unit型は特に意味のない型指定
    public IObservable<Unit> OnJoin => _onJoin;
    //OnJoin (Subject型)では2つの機能がある。値（今回はタイミング）を飛ばすのと、受け取るの。
    //飛ばす側の例_onJoin.OnNext(hoge);主にOnNextを使う。一応他に、 OnCompletedがあり、それを出すと受け取る側も受け取り待機をやめる。
    //受け取る側の例_matchingManager.OnJoin.Subscribe(_ => sceneLoader.LoadAvatarSelect().Forget()).AddTo(gameObject);(AvatarSelectシーンを読み込む時の例)
    public override void OnJoinedRoom()//部屋に入ったとき
    {
        Room = PhotonNetwork.CurrentRoom;
        Debug.Log($"Join {Room.Name}");
        Player = PhotonNetwork.LocalPlayer;
        var debugParameter = Resources.Load<DebugParameter>("DebugParameter");
        if (IsHost)
        {
            var isUseVoice = debugParameter.EventType == 0 || debugParameter.IsUseVoiceChat;//normalタイプの時か、ボイスチャット使うって明示されてる時
            debugParameter.VoiceChatSwitch(isUseVoice);
            var photonView = GetComponent<PhotonView>();
            photonView.RPC(nameof(PlayPhotonVoice), RpcTarget.Others, isUseVoice);
            if (isUseVoice)
            {
                PhotonNetwork.Instantiate("PhotonVoice", Vector3.zero, Quaternion.identity);
            }
        }
        if (!IsHost)
            _connectedAudio.Play();//よくない
        _onJoin.OnNext(Unit.Default);
    }
    [PunRPC]
    void PlayPhotonVoice(bool isUseVoice)
    {//guest側の処理
        var debugParameter = Resources.Load<DebugParameter>("DebugParameter");
        if (isUseVoice)
        {
            PhotonNetwork.Instantiate("PhotonVoice", Vector3.zero, Quaternion.identity);
        }
        debugParameter.VoiceChatSwitch(isUseVoice);
    }

    Subject<Unit> _onEnterRoom = new Subject<Unit>();
    public IObservable<Unit> OnEnterRoom => _onEnterRoom;
    public override void OnPlayerEnteredRoom(Player newPlayer)//部屋に新しく人が入ったとき
    {
        var type = IsHost ? "Host" : "Guest";
        Debug.Log($"Another Player Join as {type}");
        if (newPlayer.UserId != Player.UserId)
        {
            _connectedAudio.Play();
            _onEnterRoom.OnNext(Unit.Default);
        }
    }

    Subject<Unit> _onLeft = new Subject<Unit>();
    public IObservable<Unit> OnLeft => _onLeft;
    public override void OnPlayerLeftRoom(Player player)//他の人が（自分も？）部屋を出たとき
    {
        Debug.Log("通信切断");
        if (player.UserId == Player.UserId) return;//これはやや無駄かもしれない処理 (?)
        Debug.Log("Another Player Left");
        if (newSceneLoader.MainScene != NewScene.p6_End && OnAnotherPlayerLoadScene.Value != NewScene.p6_End)
        {
            _disconnectedAudio.Play();
            _onLeft.OnNext(Unit.Default);
            LeaveRoom();
        }
    }


    Hashtable _hash = new Hashtable();
    public void SetCustomProperties(object key, object value)
    {
        if (!_hash.ContainsKey(key))
            _hash.Add(key, value);
        else
            _hash[key] = value;
        Player.SetCustomProperties(_hash);
    }

    Subject<Avatar> _onAvatarReceive = new Subject<Avatar>();
    public IObservable<Avatar> OnAvatarReceive => _onAvatarReceive.TakeUntilDestroy(this);

    Subject<LocationData> _onLocationReceive = new Subject<LocationData>();
    public IObservable<LocationData> OnLocationReceive => _onLocationReceive.TakeUntilDestroy(this);
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {//これはExitGamesのライブラリを参照していて、3人以上のプロジェクトになるのをちょっと考慮したシステム採用。特に不要。
        if (targetPlayer.UserId == Player.UserId) return;
        foreach (var pair in changedProps)
        {
            if ((string)pair.Key == "avatar")
                _onAvatarReceive.OnNext((Avatar)pair.Value);
            else if ((string)pair.Key == "firstLocation")
            {
                var array = (string[])pair.Value;
                var locationData = new LocationData(array[0], double.Parse(array[1]), double.Parse(array[2]));
                _onLocationReceive.OnNext(locationData);
            }
        }
    }

    public GameObject InstantiatePlayerRoot()
    {
        var name = IsHost ? "HostPlayerRoot" : "GuestPlayerRoot";
        return PhotonNetwork.Instantiate(name, Vector3.zero, Quaternion.identity);
    }

    public void NoteSpawner(int noteNumber)
    {//同じノートを再生するための処理。ゲスト側に処理を送る
        if (!IsHost) return;
        var photonView = GetComponent<PhotonView>();
        photonView.RPC(nameof(NoteSpawn), RpcTarget.Others, noteNumber);
    }
    [PunRPC]
    void NoteSpawn(int noteNumber)
    {
        if (IsHost) return;
        FindObjectOfType<NoteSpawner>().AppearNote(noteNumber);
    }

    public async void LeaveRoom()//通信切れたときに、これを呼び出してアプリを終了する。特にPhotonには関係ない
    {
        await UniTask.Delay(3000);
        await FindObjectOfType<HandlePPVolume>().UpdateLensScale(0.0001f, 1);
#if UNITY_EDITOR//プラットフォームの違いの時に使う文法。
        UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
    }
}