using Cysharp.Threading.Tasks;
using UnityEngine;

public class IntroductionSound : MonoBehaviour
{
    //プレイシーンの始まりに、操作紹介の天の声を流す。
    [SerializeField]
    AudioClip _introClip;
    [SerializeField]
    float _delayTime;
    public AudioSource AudioSource{ get; private set; }
    async private void Start()
    {
        var debugParameter = Resources.Load<DebugParameter>("DebugParameter");
        if(debugParameter.EventType == 1) return;
        await UniTask.Delay((int)(_delayTime * 1000));
        AudioSource = GetComponent<AudioSource>();
        if (AudioSource != null)
        {
            AudioSource.PlayOneShot(_introClip);
        }
        else
        {
            Debug.LogAssertion("AudioSourceが見つかりません。");
        }
    }
}
