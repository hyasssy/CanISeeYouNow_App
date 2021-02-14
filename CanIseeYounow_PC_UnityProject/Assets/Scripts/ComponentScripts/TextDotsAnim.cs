using System;
using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;

public class TextDotsAnim : MonoBehaviour
{
    double _timeSpan = 0.05;
    private int amount = 77;
    private void Start()
    {
        DotsAnim(this.GetCancellationTokenOnDestroy()).Forget();
    }

    async UniTask DotsAnim(CancellationToken token)
    {
        Text text = GetComponent<Text>();
        int count = 0;
        while (true)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(_timeSpan), cancellationToken:token);
            if (count < amount)
            {
                count++;
                text.text += ".";
            }
            else
            {
                count = 0;
                text.text = "";
            }
        }
    }
}