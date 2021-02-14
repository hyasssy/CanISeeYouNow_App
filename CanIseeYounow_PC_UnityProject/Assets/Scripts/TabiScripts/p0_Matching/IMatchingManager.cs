using UnityEngine;
using System;
using UniRx;

public interface IMatchingManager
{
    bool IsHost { get; }
    IObservable<Unit> OnJoin { get; }

    IObservable<Unit> OnEnterRoom { get; }

    void JoinOrCreateRoom(string roomName);

    void SetCustomProperties(object key, object value);
    IObservable<Avatar> OnAvatarReceive { get; }

    GameObject InstantiatePlayerRoot();

    IObservable<LocationData> OnLocationReceive { get; }
    IReadOnlyReactiveProperty<NewScene> OnAnotherPlayerLoadScene { get; }
    void LeaveRoom();
}
