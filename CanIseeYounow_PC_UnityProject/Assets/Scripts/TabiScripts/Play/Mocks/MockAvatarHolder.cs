using UnityEngine;

public class MockAvatarHolder : MonoBehaviour, IAvatarHolder
{
    [SerializeField]
    Avatar _avatar = Avatar.UnityChan;
    public Avatar HostAvatar => _avatar;
    public Avatar GuestAvatar => _avatar;
    public Avatar MyAvatar => _avatar;
}
