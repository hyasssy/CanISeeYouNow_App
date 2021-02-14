using System;
using UnityEngine;
using UnityEngine.Networking;
using Cysharp.Threading.Tasks;
using UniRx;

public enum MetaDataStatus
{
    OK,
    NoData,
    NoMoney,
    Other,
}

public static class StreetViewAPI
{
    static Subject<MetaDataStatus> _statusStream = new Subject<MetaDataStatus>();
    public static IObservable<MetaDataStatus> StatusStream => _statusStream;

    public static UniTask<LocationData> GetLocationData(double lat, double lng)
    {
        return GetLocationData($"{lat},{lng}");
    }
    public static async UniTask<LocationData> GetLocationData(string location)
    {
        var debugParameter = Resources.Load<DebugParameter>("DebugParameter");
        var apiKey = debugParameter.APIkey;
        var url = $"https://maps.googleapis.com/maps/api/streetview/metadata?location={location}&key={apiKey}";
        using (var uwr = UnityWebRequest.Get(url))
        {
            var result = await uwr.SendWebRequest();
            if (result.isNetworkError)
            {
                throw new Exception("Network Error");
            }
            var metaData = JsonUtility.FromJson<MetaDataJson>(result.downloadHandler.text);

            switch (metaData.Status)
            {
                case "OK":
                    return metaData.LocationData;
                case "ZERO_RESULTS":
                case "NOT_FOUND":
                    _statusStream.OnNext(MetaDataStatus.NoData);
                    break;
                default:
                    _statusStream.OnNext(MetaDataStatus.Other);
                    break;
            }
            throw new Exception(metaData.Status);
        }
    }
    public static async UniTask<Texture2D> GetTexture(int width, int height, string panoId, double heading, double pitch, string source)
    {
        var debugParameter = Resources.Load<DebugParameter>("DebugPArameter");
        var apiKey = debugParameter.APIkey;
        var url = $"https://maps.googleapis.com/maps/api/streetview?size={width}x{height}&pano={panoId}&heading={heading}&pitch={pitch}&fov=90&source={source}&key={apiKey}";
        using (var uwr = UnityWebRequestTexture.GetTexture(url))
        {
            var result = await uwr.SendWebRequest();
            if (result.isNetworkError)
            {
                _statusStream.OnNext(MetaDataStatus.Other);
                throw new Exception("Network Error");
            }
            try
            {
                var data = ((DownloadHandlerTexture)result.downloadHandler).texture;
                _statusStream.OnNext(MetaDataStatus.OK);
                return data;
            }
            catch
            {
                debugParameter.RemoveKey(apiKey);
                if (debugParameter.CanUse)
                    return await GetTexture(width, height, panoId, heading, pitch, source);
                else
                {
                    _statusStream.OnNext(MetaDataStatus.NoMoney);
                    throw new Exception("No Money Error");
                }
            }
        }
    }
}
