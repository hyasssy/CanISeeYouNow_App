using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

///<summary>
///fade in/out animation
///</summary>
public static class UIAnim
{
    public const float DURATION = 1f;//所要時間
    public static void FadeInOut(GameObject root, bool isFadeIn)
    {
        var start = isFadeIn ? 0f : 1f;
        var end = isFadeIn ? 1f : 0f;

        var text = root.GetComponent<Text>();
        var image = root.GetComponent<Image>();
        if (text != null)
        {
            var c = text.color;
            c.a = start;
            text.color = c;
            if (isFadeIn)
            {
                DOTween.ToAlpha(
                    () => text.color,
                    color => text.color = color,
                    end,
                    DURATION
                );
            }
            else
            {
                DOTween.ToAlpha(
                    () => text.color,
                    color => text.color = color,
                    end,
                    DURATION
                );
            }
        }
        else if (image != null)
        {
            var c = image.color;
            c.a = start;
            image.color = c;
            if (isFadeIn)
            {
                DOTween.ToAlpha(
                    () => image.color,
                    color => image.color = color,
                    end,
                    DURATION
                );
            }
            else
            {
                DOTween.ToAlpha(
                    () => image.color,
                    color => image.color = color,
                    end,
                    DURATION
                );
            }
        }
    }
}
