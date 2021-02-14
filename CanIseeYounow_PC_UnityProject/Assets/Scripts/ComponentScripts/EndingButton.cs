using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class EndingButton : MonoBehaviour
{
    [SerializeField]
    Button _restartBtnJap, _restartBtnEng;
    [SerializeField]
    Vector3 _endValue, _startValue;
    float _setRestartConfirmDuration = 1f;

    public void PushEnding()
    {
        _restartBtnJap.interactable = false;
        _restartBtnEng.interactable = false;
        transform.DOMove(_endValue, _setRestartConfirmDuration, false);
    }
    public void PushYes()
    {
        FindObjectOfType<SessionManager>().LoadEndScene();
    }
    async public void PushNo()
    {
        transform.DOMove(_startValue, _setRestartConfirmDuration, false);
        await UniTask.Delay((int)(_setRestartConfirmDuration * 1000));//連打対策
        _restartBtnJap.interactable = true;
        _restartBtnEng.interactable = true;
    }
}
