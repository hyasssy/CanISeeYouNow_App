using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using System;

public class WhiteOutFade : MonoBehaviour
{
    [SerializeField]
    GameObject _colorPanel;
    public async UniTask WhiteOut()
    {
        _colorPanel.SetActive(true);
        FadeInOut.FadeIn(_colorPanel.transform, 1.15f, Ease.InCubic);
        await UniTask.Delay(TimeSpan.FromSeconds(1.15f));
    }
}
