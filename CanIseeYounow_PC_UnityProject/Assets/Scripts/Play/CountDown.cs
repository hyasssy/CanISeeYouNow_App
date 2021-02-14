using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CountDown : MonoBehaviour
{
    AudioSource _audioSource;
    [SerializeField]
    AudioClip _timeCountSound, _disconnectedSound;
    [Tooltip("クリップの順番で12345"), SerializeField]
    AudioClip[] _countDownSounds;
    [SerializeField]
    AudioSource _environmentSound;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }
    //カウントダウンの表示・音
    public async UniTask CountDownTask(Text text, float leftTime)
    {
        text.gameObject.SetActive(true);
        var time = leftTime;
        var flag = (int)time;
        while (time > 0)
        {
            if (flag != (int)time)
            {
                flag = (int)time;
                //_audioSource.PlayOneShot (_timeCountSound);
                if (flag < 5) _audioSource.PlayOneShot(_countDownSounds[flag]);
            }
            if (time <= 5)
            {
                var param = time - (float)(int)time;
                param = Easing.SineOut(param, 1f, 0f, 1f);
                text.color = Color.Lerp(Color.white, Color.red, param);
            }
            text.text = "Time limit : " + time.ToString("f2");

            time -= Time.deltaTime;
            await UniTask.Yield(PlayerLoopTiming.Update);
        }
        time = 0.00f;
        text.text = time.ToString("f2");
        text.color = Color.red;
        _environmentSound.Pause();
        _audioSource.PlayOneShot(_disconnectedSound);
    }
}