using System;
using UnityEngine;
using UnityEngine.Networking;
using Cysharp.Threading.Tasks;

public static class MapAPI
{
    public static UniTask<Texture2D> LoadMapTexture(Vector2 size, double lat, double lng, int zoom)
    {
        return LoadMapTexture(size, $"{lat},{lng}", zoom);
    }
    public static async UniTask<Texture2D> LoadMapTexture(Vector2 size, string location, int zoom)
    {
        var debugParameter = Resources.Load<DebugParameter>("DebugParameter");
        var url = $"https://maps.googleapis.com/maps/api/staticmap?size={size.x}x{size.y}&center={location}&markers=size:mid|color:red|{location}&zoom={zoom}&key={debugParameter.APIkey}";
        using (var uwr = UnityWebRequestTexture.GetTexture(url))
        {
            var result = await uwr.SendWebRequest();
            if (result.isNetworkError)
            {
                throw new Exception("Network Error");
            }
            return ((DownloadHandlerTexture)result.downloadHandler).texture;
        }
    }
}
