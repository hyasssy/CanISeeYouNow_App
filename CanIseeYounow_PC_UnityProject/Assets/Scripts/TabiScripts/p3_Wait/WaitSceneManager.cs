using UnityEngine;
using Cysharp.Threading.Tasks;

public class WaitSceneManager : MonoBehaviour
{
    async void Start()
    {
        var locationHolder = FindObjectOfType<FirstLocationHolder>();
        var avatarHolder = FindObjectOfType<NewAvatarHolder>();
        var sceneLoder = FindObjectOfType<NewSceneLoader>();
        await UniTask.WaitUntil(() => avatarHolder.HostAvatar != Avatar.None && avatarHolder.GuestAvatar != Avatar.None);
        await UniTask.WaitUntil(() => locationHolder.HostLocation != null && locationHolder.GuestLocation != null);
        await sceneLoder.LoadPlay();
    }
}
