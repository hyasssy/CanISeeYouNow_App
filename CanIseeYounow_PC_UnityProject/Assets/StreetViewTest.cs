using System;
using UnityEngine;
using UnityEngine.Networking;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine.UI;

public class StreetViewTest : MonoBehaviour
{
    [SerializeField]
    Image image;
    private void Awake()
    {
        // var value = CryptExpansion.Encrypt();
        // Debug.Log(value);
        // var result =
    }
    async private UniTask Start()
    {
        var result = await CheckPassword.CollatePassWord(/*_password.text*/"pass", (progress) => Debug.Log("進捗(0-1)=" + progress));
        var locationData = await StreetViewAPI.GetLocationData("35.70140757816516,139.4838876444111");
        var tex = await StreetViewAPI.GetTexture(512, 512, locationData.PanoId, 0, 0, "outdoor");
        image.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.zero);
    }
}
