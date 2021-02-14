using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;
using System;

public class InputHandler : MonoBehaviour, ILocationPusher
{
    [SerializeField] KeyCode _sceneResetKey = KeyCode.Return;
    [SerializeField] KeyCode _goKey = KeyCode.G, _backKey = KeyCode.B;

    ReactiveProperty<LocationData> _locationData = new ReactiveProperty<LocationData>();
    public IReadOnlyReactiveProperty<LocationData> LocationData => _locationData;

    public void Start()
    {
        var updateManager = FindObjectOfType<UpdateManager>();
        Debug.Log("UpdateManager取得");
        var firstLocationHolder = FindObjectOfType<FirstLocationHolder>();
        Debug.Log("FirstLocationHolder取得");
        var locationGetter = new LocationGetter(firstLocationHolder.MainPlayerLocation);
        _locationData.Value = firstLocationHolder.MainPlayerLocation;
        updateManager.InputUpdate
            .Subscribe(async _ =>
            {
                //戻る
                if (Input.GetKeyDown(_backKey))
                {
                    var data = locationGetter.MoveBack();
                    _locationData.Value = data;
                }

                //進む
                if (Input.GetKeyDown(_goKey))
                {
                    var data = await locationGetter.MoveNext();
                    _locationData.Value = data;
                }
            });
    }
}