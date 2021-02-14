using System;
using UnityEngine;
using Cysharp.Threading.Tasks;
using DG.Tweening;

public class FadeComponent : MonoBehaviour
{
    //fadeinのアニメーションするだけのコンポーネント
    public enum FadeType//必要に応じて増やしてもよい。
    {
        FadeInAppear,
        PanelFadeOutStart,
        FadeInScaleUpAppear
    }
    [SerializeField]
    FadeType _fadeType;
    [SerializeField]
    float _delay = 0;

    async private void Start()
    {
        switch ((int)_fadeType)
        {
            case 0://子オブジェクト含めてfadein
                FadeInOut.FadeIn(transform, 1, Ease.OutQuad, _delay);
                break;
            case 1:
                FadeInOut.FadeOut(transform, 1, Ease.OutQuad, _delay);
                break;
            case 2://fadeinしてスケールも大きくする
                transform.localScale = Vector3.zero;
                await UniTask.Delay(TimeSpan.FromSeconds(_delay));
                FadeInOut.FadeIn(transform, 2f, Ease.InOutCubic);//子オブジェクト含めてfadein
                transform.DOScale(1, 1.5f).SetEase(Ease.InOutCubic);
                break;
            default: break;
        }
    }

    async public UniTask FadeOutScaleUp()
    {
        FadeInOut.FadeOut(transform, 1.15f, Ease.InCubic);
        transform.DOScale(4, 1.3f).SetEase(Ease.InCubic);
        await UniTask.Delay(TimeSpan.FromSeconds(2));
    }
}
