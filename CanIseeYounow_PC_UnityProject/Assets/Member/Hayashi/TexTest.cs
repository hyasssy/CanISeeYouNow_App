using Cysharp.Threading.Tasks;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class TexTest : MonoBehaviour
{
    [SerializeField]
    RenderTexture _tex;
    [SerializeField]
    Texture2D _t2;
    [SerializeField]
    Image _image;
    [SerializeField]
    GameObject _camera;

    async void Start()
    {
        //var currentRT = RenderTexture.active;
        //RenderTexture.active = _tex;
        await UniTask.Yield();
        _camera.SetActive(false);
        //RenderTexture.active = currentRT;
        var texture = new Texture2D(_tex.width, _tex.height);
        texture.ReadPixels(new Rect(0, 0, texture.width, texture.height), 0, 0);
        texture.Apply();
        byte[] bytes = texture.EncodeToPNG();
        File.WriteAllBytes("Assets/RenderTexture.png", bytes);
        print("save");
        _image.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
    }
}