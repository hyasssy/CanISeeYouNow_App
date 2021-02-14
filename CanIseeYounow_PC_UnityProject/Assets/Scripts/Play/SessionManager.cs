using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;
using UniRx;
using Photon.Voice.PUN;
using System.Linq;

public class SessionManager : MonoBehaviour
{
    //見えるようになる
    //出会った時の演出
    //カウントダウン、音が乱れ、画面にノイズが入っていく
    //Disconnected
    //BlackOut
    [SerializeField]
    GameObject[] _uigroups;
    [SerializeField]
    GameObject _fireWorksPrefab;
    [SerializeField]
    GameObject endingButton, textToTellEnd;
    [SerializeField]
    Image _blackPanel;

    [SerializeField]
    AudioSource _tuneAudio = default;
    float _fireWorksTime = 10f;
    double _extentionTime = 2;
    [SerializeField]
    GameObject _meetText = default;
    [SerializeField]
    AudioSource _dissconnectedSound = default;
    [SerializeField]
    AudioSource _environmentalSound = default;
    DebugParameter _debugParameter;
    [SerializeField]
    GameObject _endingButton;
    public bool IsMeeted { get; private set; } = false;

    private void Start() {
        _debugParameter = Resources.Load<DebugParameter>("DebugParameter");
    }

    async UniTask AutoDestroy(GameObject obj, float delay)
    {
        await UniTask.Delay(TimeSpan.FromSeconds((double)delay), false, PlayerLoopTiming.Update);
        print("FadeOutさせる");
        //FadeOutさせる
        Destroy(obj);
    }


    ReactiveProperty<bool> _meeted = new ReactiveProperty<bool>(false);
    public IReadOnlyReactiveProperty<bool> Meeted => _meeted;
    public async UniTask Meet()
    {
        if (Meeted.Value) return;
        //あった時の演出
        _meeted.Value = true;
        IsMeeted = true;
        foreach (GameObject obj in _uigroups) obj.SetActive(false);
        // // //fireworks
        FireWorks.Fire(_fireWorksPrefab, _fireWorksTime, Camera.main.transform).Forget();
        _meetText.SetActive(true);
        await UniTask.Delay(TimeSpan.FromSeconds(5), false, PlayerLoopTiming.Update);
        //エンディングボタンを表示する。
        textToTellEnd.SetActive(true);
        endingButton.SetActive(true);
        Debug.Log("Endingボタンを表示");
        await UniTask.Delay(TimeSpan.FromSeconds(7), false, PlayerLoopTiming.Update);
        _meetText.SetActive(false);
        // // //曲鳴らす
        _tuneAudio.Play();
    }

    async UniTask LoadEndSceneTask()
    {
        await UniTask.Delay(1000);
        Debug.Log("Endいきた");
        if(_debugParameter.EventType == 0 || _debugParameter.IsUseVoiceChat){
        var audioSources = FindObjectsOfType<PhotonVoiceView>().Select(v => v.GetComponent<AudioSource>());
        foreach (var source in audioSources)
            source.mute = true;
        }
        if(_environmentalSound != null) _environmentalSound.mute = true;
        _dissconnectedSound.Play();
        await UniTask.Delay(TimeSpan.FromSeconds(_extentionTime));
        FindObjectOfType<LatLngText>().gameObject.SetActive(false);
        ZoomOut(2f).Forget();
        await UniTask.Delay(TimeSpan.FromSeconds(1f));
        _blackPanel.gameObject.SetActive(true);
        await FadeInOut.FadeImage(_blackPanel, 0f, 1f, 2f);
        await FindObjectOfType<NewSceneLoader>().LoadEnd();
    }
    public void LoadEndScene(){
        _endingButton.SetActive(false);
        LoadEndSceneTask().Forget();
    }

    //シーン遷移前のアニメーション。pp>LensDistortion>scaleをいじる
    async UniTask ZoomOut(float duration)
    {
        PostProcessVolume ppv = FindObjectOfType<PostProcessVolume>();
        LensDistortion ld = ppv.profile.GetSetting<LensDistortion>();
        var startScale = 1f;
        var endScale = 0f;
        var param = 0f;

        while (param < duration)
        {
            param += Time.deltaTime;
            ld.scale.Interp(startScale, endScale, Easing.QuadIn(param / duration, 1f, 0f, 1f));
            await UniTask.Yield(PlayerLoopTiming.Update);
        }
    }
}