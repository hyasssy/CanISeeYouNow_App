using UnityEngine;
using UniRx;

public interface IAvatarHolder
{
    Avatar HostAvatar { get; }
    Avatar GuestAvatar { get; }
    Avatar MyAvatar { get; }
}

public class NewAvatarHolder : MonoBehaviour, IAvatarHolder
{
    public Avatar HostAvatar { get; private set; }
    public Avatar GuestAvatar { get; private set; }
    public Avatar MyAvatar => _matchingManager.IsHost ? HostAvatar : GuestAvatar;
    IMatchingManager _matchingManager;
    void Start()
    {
        var matchingManager = FindObjectOfType<PhotonMatchingManager>();
        DontDestroyOnLoad(gameObject);
        _matchingManager = matchingManager;
        var sceneLoder = FindObjectOfType<NewSceneLoader>();
        _matchingManager.OnAvatarReceive
            .Subscribe(x =>
            {
                if (_matchingManager.IsHost)
                    GuestAvatar = x;
                else
                    HostAvatar = x;
                Debug.Log($"Guest Avatar name is {GuestAvatar}");
            });
        sceneLoder.OnLoad
            .Where(x => x == NewScene.p1_Matching)
            .Subscribe(_ => Destroy(gameObject))
            .AddTo(this);
    }

    public async void SetAvatar(Avatar avatar)
    {
        if (MyAvatar != Avatar.None) return;
        if (_matchingManager.IsHost)
            HostAvatar = avatar;
        else
            GuestAvatar = avatar;
        _matchingManager.SetCustomProperties("avatar", avatar);

        if (_matchingManager.IsHost)
            await FindObjectOfType<NewSceneLoader>().LoadLocationInput();
        else
            await FindObjectOfType<NewSceneLoader>().LoadWait();
    }
}
