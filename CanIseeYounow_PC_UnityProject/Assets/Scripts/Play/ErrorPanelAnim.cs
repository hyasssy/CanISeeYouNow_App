using UnityEngine;
using System;
using Cysharp.Threading.Tasks;
using System.Threading;

///<summary>
///ErrorPanelのアニメーションを制御
///</summary>
public class ErrorPanelAnim : MonoBehaviour
{
    //[SerializeField]
    float _minDelay = 0.7f, _maxDelay = 3.5f;
    //[SerializeField]
    float _longProbability = 0.3f;
    [SerializeField]
    string _shortGlitchAnimName, _longGlitchAnimName;
    [SerializeField]
    AudioClip _shortGlitchNoise, _longGlitchNoise;
    Animator _anim;
    AudioSource _audioSource;
    CancellationTokenSource _cancellationTokenSource;
    private void OnEnable()
    {
        _anim = GetComponent<Animator>();
        _audioSource = GetComponent<AudioSource>();
        _cancellationTokenSource = new CancellationTokenSource();
        Task(_cancellationTokenSource.Token).Forget();
    }

    private void OnDisable()
    {
        _cancellationTokenSource.Cancel();
    }

    async UniTask Task(CancellationToken cancellation_token)
    {
        while (true)
        {
            float delay = UnityEngine.Random.Range(_minDelay, _maxDelay);
            //await UniTask.Yield(PlayerLoopTiming.Update, cancellation_token);
            await UniTask.Delay(TimeSpan.FromSeconds(delay), true, PlayerLoopTiming.Update, cancellation_token);
            float randomNum = UnityEngine.Random.Range(0f, 1f);
            if (randomNum < _longProbability)
            {
                _anim.SetTrigger(_longGlitchAnimName);
                _audioSource.PlayOneShot(_longGlitchNoise);
                print("L");
            }
            else
            {
                _anim.SetTrigger(_shortGlitchAnimName);
                _audioSource.PlayOneShot(_shortGlitchNoise);
                print("S");
            }
        }
    }
}
