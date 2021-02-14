using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class LocationGetter {
    int _maxRange = 12;
    int _moveDistance = 2;
    LocationCasher _locationCasher;
    Camera _camera;

    public LocationGetter (LocationData locationData) {
        CreateLoactonCasher (locationData);
        _camera = Camera.main;
    }

    async void CreateLoactonCasher (LocationData locationData) {
        if (locationData.PanoId != null)
            _locationCasher = new LocationCasher (locationData);
        else {
            _locationCasher = new LocationCasher (await StreetViewAPI.GetLocationData (locationData.Lat, locationData.Lng));
        }
    }

    public async UniTask<LocationData> MoveNext () {
        var clickVector = GetClickVector (_camera);
        var currentLocation = _locationCasher.CurrentLocation;

        LocationData newData = null;
        for (int range = _moveDistance; range <= _maxRange; range += _moveDistance) {
            // クリックした方向5m分をその地点の緯度をベースに緯度経度に変換し、現在の座標に加算
            var (targetLat, targetLng) = CoordinateExt.GetLatLng (currentLocation, range, clickVector);
            try {
                newData = await StreetViewAPI.GetLocationData (targetLat, targetLng);
            } catch (Exception e) {
                Debug.LogWarning (e.Message);
                continue;
            }

            if (currentLocation.PanoId != newData.PanoId) {
                return _locationCasher.AddCash (newData);
            }
        }
        Debug.LogError ("No Location Data");
        return null;
    }

    public LocationData MoveBack () => _locationCasher.MoveBack ();

    public static Vector3 GetClickVector (Camera camera) {
        var clickedPos = camera.ScreenToWorldPoint (new Vector3 (Input.mousePosition.x, Input.mousePosition.y, camera.nearClipPlane));
        return Vector3.Scale (clickedPos - camera.transform.position, new Vector3 (1, 0, 1)).normalized;
    }
}