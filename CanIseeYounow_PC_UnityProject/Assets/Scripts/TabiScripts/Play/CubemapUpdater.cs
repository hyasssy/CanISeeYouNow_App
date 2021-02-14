using UnityEngine;
using Cysharp.Threading.Tasks;
using UniRx;
using System.Collections.Generic;

public class CubemapUpdater : MonoBehaviour
{
    Cubemap _cubemap;
    Material _groundMaterial;

    public void Start()
    {
        _cubemap = Resources.Load<Cubemap>("tempGSVmap");
        _groundMaterial = Resources.Load<Material>("GroundMat");
        var updateManager = FindObjectOfType<UpdateManager>();
        var inputHandler = FindObjectOfType<InputHandler>();
        var firstLocationHolder = FindObjectOfType<FirstLocationHolder>();
        Initialize(updateManager, firstLocationHolder.MainPlayerLocation);
        inputHandler.LocationData
            .Where(x => x?.PanoId != null)
            .Subscribe(x => UpdateMaterial(x).Forget());
    }

    async void Initialize(UpdateManager updateManager, LocationData locationData)
    {
        var data = await StreetViewAPI.GetLocationData(locationData.Lat, locationData.Lng);
        await UpdateMaterial(data);
        updateManager.paused.Value = false;
        Debug.Log($"Pause {updateManager.paused.Value}");
    }


    TextureCasher _casher = new TextureCasher();

    //解体する
    async UniTask UpdateMaterial(LocationData locationData)
    {
        var texs = await _casher.GetTexs(locationData);
        SetTexture(texs);
    }

    void SetTexture((CubemapFace, Texture2D)[] texs)
    {
        foreach (var (face, tex) in texs)
        {
            if (face == CubemapFace.PositiveY)
            {
                // これがグラウンド。
                _groundMaterial.mainTexture = tex;
            }

            var cubeMapColors = tex.GetPixels();
            _cubemap.SetPixels(cubeMapColors, face);
        }

        _cubemap.Apply();
    }
}