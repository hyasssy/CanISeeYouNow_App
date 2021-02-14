using System;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UnityEngine.UI;
using UniRx.Triggers;
using System.Linq;
using Cysharp.Threading.Tasks;

public class AvatarSelecter : MonoBehaviour
{
    [SerializeField]
    Transform _avatarParent;
    [SerializeField]
    Transform _buttonParent;


    async void Start()
    {
        var allAvatar = Enum.GetValues(typeof(Avatar)).Cast<Avatar>().ToList();
        allAvatar.Remove(Avatar.None);
        var random = new System.Random();
        var avatars = allAvatar.OrderBy(_ => random.Next()).Take(3);
        var holder = FindObjectOfType<NewAvatarHolder>();
        (await Initialize(avatars))
            .Do(x => Debug.Log($"Select {x} Avatar"))
            .Subscribe(x => holder.SetAvatar(x));
    }

    async UniTask<IObservable<Avatar>> Initialize(IEnumerable<Avatar> selectableAvatars)
    {
        var list = new List<IObservable<Avatar>>();
        var i = 0;
        foreach (var avatar in selectableAvatars)
        {
            var avatarParent = _avatarParent.GetChild(i).GetChild(0);
            await avatar.InstantiateAsync(avatarParent);
            var stream = _buttonParent.GetChild(i).GetComponent<Button>()
                .OnPointerClickAsObservable()
                .Select(_ => avatar);
            list.Add(stream);
            i++;
        }
        return Observable.Merge(list).TakeUntilDestroy(this);
    }
}
