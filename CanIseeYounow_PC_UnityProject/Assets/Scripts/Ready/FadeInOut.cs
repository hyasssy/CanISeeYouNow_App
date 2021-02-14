using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using System;

public class FadeInOut
{
    const Ease FADEINEASE = Ease.OutQuad;
    const Ease FADEOUTEASE = Ease.OutQuad;
    const Ease SCALEUPEASE = Ease.OutQuad;
    const Ease SCALEDOWNEASE = Ease.OutQuad;
    const float FADEINDURATION = 1f;
    const float FADEOUTDURATION = 1f;
    const float MAXSIZE = 5f;
    public async static UniTask FadeInTask(Transform parent, float duration = FADEINDURATION, Ease ease = FADEINEASE, float delay = 0)
    {
        var graphics = parent.GetComponentsInChildren<Graphic>();
        foreach (var x in graphics)
        {
            var color = x.color;
            color.a = 0;
            x.color = color;
        }
        await UniTask.Delay(TimeSpan.FromSeconds((double)delay));
        await Task.WhenAll(graphics.Select(x => x.DOFade(1, duration)
          .SetEase(ease).AsyncWaitForCompletion()));
    }
    public async static UniTask FadeOutTask(Transform parent, float duration = FADEOUTDURATION, Ease ease = FADEOUTEASE, float delay = 0)
    {
        var graphics = parent.GetComponentsInChildren<Graphic>();
        foreach (var x in graphics)
        {
            var color = x.color;
            color.a = 1;
            x.color = color;
        }
        await UniTask.Delay(TimeSpan.FromSeconds((double)delay));
        await Task.WhenAll(graphics.Select(x => x.DOFade(0, duration)
          .SetEase(ease).AsyncWaitForCompletion()));
    }

    ///<summary>
    ///unitask参照せずともFadeInTaskを使用できるメソッド。
    ///</summary>
    public static void FadeIn(Transform parent, float duration = FADEINDURATION, Ease ease = FADEINEASE, float delay = 0)
    {
        FadeInTask(parent, duration, ease, delay).Forget();
    }
    ///<summary>
    ///unitask参照せずともFadeOutTaskを使用できるメソッド。
    ///</summary>
    public static void FadeOut(Transform parent, float duration = FADEOUTDURATION, Ease ease = FADEINEASE, float delay = 0)
    {
        FadeOutTask(parent, duration, ease, delay).Forget();
    }
    public async static UniTask Scale0to1(Transform parent, float duration = FADEINDURATION, Ease ease = SCALEUPEASE, float delay = 0)
    {
        await UniTask.Delay(TimeSpan.FromSeconds((double)delay));
        parent.localScale = Vector3.zero;
        parent.DOScale(Vector3.one, duration).SetEase(ease);
    }
    public async static UniTask Scale1toMax(Transform parent, float duration = FADEOUTDURATION, Ease ease = SCALEDOWNEASE, float delay = 0)
    {
        await UniTask.Delay(TimeSpan.FromSeconds((double)delay));
        parent.localScale = Vector3.one;
        parent.DOScale(Vector3.one * MAXSIZE, duration).SetEase(ease);
    }

    public static async UniTask FadeImage(Graphic graphic, float startAlpha, float endAlpha, float duration)
    {
        var timeCount = 0f;
        Color primaryColor = graphic.color;
        while (timeCount < duration)
        {
            timeCount += Time.deltaTime;
            Color color = primaryColor;
            color.a = Mathf.Lerp(startAlpha, endAlpha, timeCount / duration);
            graphic.color = color;
            await UniTask.Yield(PlayerLoopTiming.Update);
        }
    }
}