using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;

public class UrlButton : MonoBehaviour
{
    const string URL = "https://www.google.co.jp/maps/?hl=ja";

    public void OpenURL()
    {
        Application.OpenURL(URL);
    }
}
