using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;

public interface IFirstLocationHolder
{
    LocationData HostLocation { get; }
    LocationData GuestLocation { get; }
    LocationData MainPlayerLocation { get; }
}

public class FirstLocationHolder : MonoBehaviour, IFirstLocationHolder
{
    public LocationData HostLocation { get; private set; }
    public LocationData GuestLocation { get; private set; }
    public LocationData MainPlayerLocation => _matchingManager.IsHost ? HostLocation : GuestLocation;

    IMatchingManager _matchingManager;
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        _matchingManager = FindObjectOfType<PhotonMatchingManager>();
        var reach = Resources.Load<DebugParameter>("DebugParameter").FirstLocationReach;
        _matchingManager.OnLocationReceive
            .Do(x => Debug.Log($"Another LocationData is {x.Lat} {x.Lng}"))
            .Subscribe(async x =>
            {
                if (_matchingManager.IsHost)
                    GuestLocation = x;
                else
                {//ゲストの時ちょっと離れたところにする
                    HostLocation = x;
                    GuestLocation = await GetGuestLocation(HostLocation, reach);
                    Debug.Log("Send Guest LocationData");
                    _matchingManager.SetCustomProperties("firstLocation", LocationToStringArray(GuestLocation));
                }
            });
        _matchingManager.OnAnotherPlayerLoadScene
            .Where(_ => _matchingManager.IsHost)
            .Where(x => x == NewScene.p4_Wait)
            .Where(_ => HostLocation != null)
            .Subscribe(_ => SetFirstLocation(HostLocation));

        FindObjectOfType<NewSceneLoader>().OnLoad
            .Where(x => x == NewScene.p1_Matching)
            .Subscribe(_ => Destroy(gameObject))
            .AddTo(this);
    }
    int flag = 0;
    async UniTask<LocationData> GetGuestLocation(LocationData hostLocation, int reach){
        try{
            var dir = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;
            var (lat, lng) = CoordinateExt.GetLatLng(hostLocation, reach, dir);

            var value = await StreetViewAPI.GetLocationData(lat, lng);
            return value;
        }
        catch{
            flag++;
            if(flag < 50){//たくさんトライできるようにしとく。
            var value = await GetGuestLocation(hostLocation, reach);
            return value;
            }
            Debug.LogAssertion("データ取れません。");
            return null;
        }
    }

    public void SetFirstLocation(LocationData locationData)
    {
        if (_matchingManager.IsHost)
        {
            HostLocation = locationData;
            _matchingManager.SetCustomProperties("firstLocation", LocationToStringArray(HostLocation));
        }
        else
        {
            GuestLocation = locationData;
        }
    }

    string[] LocationToStringArray(LocationData locationData)
    {
        return new string[]
            {
                HostLocation.PanoId,
                HostLocation.Lat.ToString(),
                HostLocation.Lng.ToString(),
            };
    }
}
