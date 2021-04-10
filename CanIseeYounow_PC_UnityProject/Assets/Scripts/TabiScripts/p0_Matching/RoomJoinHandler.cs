using UnityEngine;
using UnityEngine.UI;
using UniRx;
using Cysharp.Threading.Tasks;

public class RoomJoinHandler : MonoBehaviour
{
    [SerializeField] InputField _roomNameInput;
    // [SerializeField] InputField _password;

    [SerializeField] Button _joinButtonNormal; //UniRXの機能で、ボタンをここに参照させると、オブザーバブルに設定できる。
    [SerializeField] Button _joinButtonShow;
    [SerializeField] PhotonMatchingManager _matchingManager;
    [SerializeField] GameObject _passwordCaution;
    bool flag = false;


    void Start()
    {
        _matchingManager.newSceneLoader = FindObjectOfType<NewSceneLoader>();
        Debug.Log("MatchingManager is" + _matchingManager != null);
        //ここまではマッチが切れたときに最初に戻るためにやろうとしたけどあんまりうまく行ってないところ。
        var sceneLoder = FindObjectOfType<NewSceneLoader>();
        if (Resources.Load<DebugParameter>("DebugParameter").EventType == 0)
        {
            _joinButtonNormal.OnClickAsObservable() //ボタンの設定
                .Where(_ => !string.IsNullOrWhiteSpace(_roomNameInput.text))
                .Subscribe(_ =>
                {
                    if (!_matchingManager.IsConnected) return;//ロビーにつながってなかったらリターン
                    Button[] activeButtons = FindObjectsOfType<Button>();
                    foreach (Button b in activeButtons) b.interactable = false;
                    Toggle[] activeToggles = FindObjectsOfType<Toggle>();
                    foreach (Toggle t in activeToggles) t.interactable = false;
                    _matchingManager.JoinOrCreateRoom(_roomNameInput.text); //ここでルームネームを設定して部屋を作るor入室している。
                })
                .AddTo(gameObject); //MonoBehaviorのスクリプトの中では、AddToをつけておく。これがあると、そのオブジェクトが消えた後に処理が走っちゃってバグることを防ぐことができる。（このオブジェクトが消えたらこの処理も破棄するというマーク。）
        }
        else if (Resources.Load<DebugParameter>("DebugParameter").EventType == 1)
        {
            _joinButtonShow.OnClickAsObservable()
                .Subscribe(async _ =>
                {
                    if (!_matchingManager.IsConnected) return;//ロビーにつながってなかったらリターン
                    bool result;
                    try
                    {
                        result = await CheckPassword.CollatePassWord(/*_password.text*/"pass", (progress) => Debug.Log("進捗(0-1)=" + progress));
                    }
                    catch
                    {
                        Debug.Log("パスワードチェックシステムエラー");
                        result = false;
                    }
                    if (!result)
                    {
                        // _password.text = "Input correct password";
                        _passwordCaution.SetActive(true);
                        return;
                    }
                    if (flag) return;
                    flag = true;
                    Button[] activeButtons = FindObjectsOfType<Button>();
                    foreach (Button b in activeButtons) b.interactable = false;
                    Toggle[] activeToggles = FindObjectsOfType<Toggle>();
                    foreach (Toggle t in activeToggles) t.interactable = false;
                    // var list = RoomInfo
                    _matchingManager.JoinOrCreateRoom("special");
                })
                .AddTo(gameObject);
        }

        _matchingManager.OnJoin //OnJoinに、ここに書いてある処理を登録する (UniRXの説明)。無事に部屋に入れたら次のシーンにいく。OnJoinは、サーバーに入った時
            .Subscribe(_ => sceneLoder.LoadAvatarSelect().Forget())
            .AddTo(gameObject);
    }
}