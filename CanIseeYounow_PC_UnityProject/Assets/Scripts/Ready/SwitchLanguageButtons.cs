using UnityEngine;
using DG.Tweening;
using System;
using Cysharp.Threading.Tasks;

public class SwitchLanguageButtons : MonoBehaviour
{
    Vector3 _largeFontSize, _smallFontSize;
    [SerializeField]
    GameObject[] _japObjs, _engObjs;//最初のシーンの日本語オブジェと英語オブジェの切り替えアニメーション用
    [SerializeField]
    float _duration = 0.3f;
    [SerializeField]
    Transform _japButton, _engButton;
    bool _isJapanese = true;
    bool _isOnAnim = false;
    DebugParameter _debugParameter;

    private void OnEnable()//フェイズの切り替えをactiveで制御しているため、ここはstartやawakeではなくonenable
    {
        _largeFontSize = _japButton.transform.localScale;
        _smallFontSize = _engButton.transform.localScale;
        _debugParameter = Resources.Load<DebugParameter>("DebugParameter");
    }
    public void PushJapanese()
    {
        if (!_isJapanese && !_isOnAnim) FadeAnim(true).Forget();
    }
    public void PushEnglish()
    {
        if (_isJapanese && !_isOnAnim) FadeAnim(false).Forget();
    }

    async UniTask FadeAnim(bool isJapButton)
    {
        _isJapanese = !_isJapanese;
        _isOnAnim = true;
        if (isJapButton)
        {
            _japButton.DOScale(_largeFontSize, _duration);
            _engButton.DOScale(_smallFontSize, _duration);
        }
        else
        {
            _japButton.DOScale(_smallFontSize, _duration);
            _engButton.DOScale(_largeFontSize, _duration);
        }
        if(_isJapanese){
            _debugParameter.LanguageSwitch(Language.Japanese);
        }else{
            _debugParameter.LanguageSwitch(Language.English);
        }
        Debug.Log("Language switched");

        if (_isJapanese)
        {
            foreach (GameObject obj in _engObjs) UIAnim.FadeInOut(obj, false);
            await UniTask.Delay(TimeSpan.FromSeconds((double)UIAnim.DURATION / 2), true, PlayerLoopTiming.Update);
            foreach (GameObject obj in _engObjs) obj.SetActive(false);
            foreach (GameObject obj in _japObjs)
            {
                obj.SetActive(true);
                UIAnim.FadeInOut(obj, true);
            }
        }
        else
        {
            foreach (GameObject obj in _japObjs) UIAnim.FadeInOut(obj, false);
            await UniTask.Delay(TimeSpan.FromSeconds((double)UIAnim.DURATION / 2), true, PlayerLoopTiming.Update);
            foreach (GameObject obj in _japObjs) obj.SetActive(false);
            foreach (GameObject obj in _engObjs)
            {
                obj.SetActive(true);
                UIAnim.FadeInOut(obj, true);
            }
        }
        await UniTask.Delay(TimeSpan.FromSeconds((double)UIAnim.DURATION / 2), true, PlayerLoopTiming.Update);
        _isOnAnim = false;
    }
}
