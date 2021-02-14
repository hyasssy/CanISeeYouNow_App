using System;
using UniRx;
using UnityEngine;

public class MockMatchingManager : MonoBehaviour, IMatchingManager
{
    public bool IsHost => true;

    public IObservable<Unit> OnJoin => null;

    public IObservable<Unit> OnEnterRoom => null;

    public IObservable<Avatar> OnAvatarReceive => null;

    public IObservable<LocationData> OnLocationReceive => null;

    public IReadOnlyReactiveProperty<NewScene> OnAnotherPlayerLoadScene => new ReactiveProperty<NewScene>(NewScene.p5_Play);

    public GameObject InstantiatePlayerRoot()
    {
        return Instantiate(Resources.Load<GameObject>("HostPlayerRoot"));
    }

    public void JoinOrCreateRoom(string roomName)
    {
        throw new NotImplementedException();
    }

    public void LeaveRoom()
    {
        throw new NotImplementedException();
    }

    public void SetCustomProperties(object key, object value)
    {
    }
}
