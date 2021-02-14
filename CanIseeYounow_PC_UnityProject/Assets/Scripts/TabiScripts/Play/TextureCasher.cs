using UnityEngine;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

public class TextureCasher
{
    // DebugParameter _debugParameter;

    // public TextureCasher()
    // {
    //     _debugParameter = Resources.Load<DebugParameter>("DebugParameter");
    // }

    List<LocationData> _cashedLocations = new List<LocationData>();

    Dictionary<LocationData, (CubemapFace, Texture2D)[]> _cash =
        new Dictionary<LocationData, (CubemapFace, Texture2D)[]>();

    public async UniTask<(CubemapFace, Texture2D)[]> GetTexs(LocationData locationData)
    {
        if (_cash.ContainsKey(locationData))
            return _cash[locationData];
        else
        {
            (CubemapFace, Texture2D)[] texs;
            // if (!_debugParameter.Debug)
            // {
                texs = await TextureLoader.LoadHexahedronTexture(locationData.PanoId);
            // }
            // else
            // {
            //     texs = new (CubemapFace, Texture2D)[1];
            //     texs[0] = await TextureLoader.LoadTexture(locationData.PanoId, CubemapFace.PositiveZ);
            // }

            _cash.Add(locationData, texs);
            _cashedLocations.Add(locationData);
            if (_cashedLocations.Count >= 200)
            {
                var key = _cashedLocations[0];
                _cash.Remove(key);
                _cashedLocations.RemoveAt(0);
            }

            return texs;
        }
    }
}