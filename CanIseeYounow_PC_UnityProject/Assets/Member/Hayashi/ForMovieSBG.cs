using System;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.Rendering.PostProcessing;

public class ForMovieSBG : MonoBehaviour
{//SetUpBackGroundの適当なオブジェにアタッチしてパラメータを調整して使う。
    [SerializeField]
    float _startScale;
    [SerializeField]
    float _endScale;
    [SerializeField]
    float _duration, _delay;
    private void Start()
    {
        GoIn().Forget();
    }
    async UniTask GoIn()
    {
        PostProcessVolume volume = FindObjectOfType<PostProcessVolume>();
        LensDistortion lensDistortion = volume.profile.GetSetting<LensDistortion>();
        float time = 0;
        await UniTask.Delay(TimeSpan.FromSeconds(_delay));
        while (time < _duration)
        {
            float p = Easing.SineInOut(time, _duration, 0, 1);
            lensDistortion.scale.Interp(_startScale, _endScale, p);
            time += Time.deltaTime;
            await UniTask.Yield();
        }
    }
}
