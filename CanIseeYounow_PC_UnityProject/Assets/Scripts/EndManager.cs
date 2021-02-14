using System;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading.Tasks;

public class EndManager : MonoBehaviour
{
    [SerializeField]
    GameObject[] _groups;
    [SerializeField]
    GameObject _leftButton, _rightButton, _quitButton;
    [SerializeField]
    double _delay = 1, _waitDuration = 0.7;
    int _phase = 0;
    bool _onTask = true;
    private void Start()
    {
        FadeInStart().Forget();
        print("気が向いたらキャンセレーショントークン入れて連打できるようにするほうが美しいか。");
    }
    async UniTask FadeInStart()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(_delay));
        _groups[0].SetActive(true);//初めは全て非表示にしておく。
        _rightButton.SetActive(true);
        await FadeInOut.FadeInTask(_groups[0].transform);
        _onTask = false;
    }
    public void PushRightButton()
    {
        if (_onTask) return;
        ShiftPhase(true).Forget();
    }
    public void PushLeftButton()
    {
        if (_onTask) return;
        ShiftPhase(false).Forget();
    }
    async UniTask ShiftPhase(bool isRight)
    {
        _onTask = true;
        int previousPhase = _phase;
        _phase = isRight ? _phase + 1 : _phase - 1;
        FadeOut(previousPhase).Forget();
        await UniTask.Delay(TimeSpan.FromSeconds(_waitDuration));
        ButtonDisplay(isRight);
        await FadeIn(_phase);
        _onTask = false;
    }
    async UniTask FadeOut(int previousPhase)
    {
        await FadeInOut.FadeOutTask(_groups[previousPhase].transform);
        _groups[previousPhase].SetActive(false);
    }
    async UniTask FadeIn(int newPhase)
    {
        _groups[newPhase].SetActive(true);
        await FadeInOut.FadeInTask(_groups[newPhase].transform);
    }
    void ButtonDisplay(bool isRight)
    {
        if (isRight)
            _leftButton.SetActive(true);
        else
        {
            _rightButton.SetActive(true);
        }
        if (_phase == _groups.Length - 1) {
            _rightButton.SetActive(false);
            _quitButton.SetActive(true);//最後までいったら以降終了ボタン表示。
        }
        if (_phase == 0) _leftButton.SetActive(false);
    }
    public void PushQuitPlayButton(){
        QuitPlay().Forget();
    }
    async UniTask QuitPlay(){
        await UniTask.Delay(1000);
#if UNITY_EDITOR//プラットフォームの違いの時に使う文法。
        UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
    }
}
