using UnityEngine;
using DG.Tweening;

public class SpreadLight : MonoBehaviour
{
    [SerializeField]
    float _duration = 7f, _maxSize = 1.3f;
    [SerializeField]
    string _hostRoot, _guestRoot;
    Material _material;
    float _primaryAlpha = 0.8f;
    float _endAlpha = 0f;
    public Color _lightColor;//プレイヤーによって色変えたらいいな

    private void OnEnable() {
        transform.localScale = Vector3.zero;
        _material = GetComponent<Renderer>().material;
        _lightColor.a = _primaryAlpha;
        _material.color = _lightColor;
        Invoke("Spread", 1f);
    }
    void Spread()
    {
        float maxSize;
        if(GameObject.Find(_hostRoot) != null && GameObject.Find(_guestRoot) != null){
            Vector3 hostPos = GameObject.Find(_hostRoot).transform.position;
            Vector3 guestPos = GameObject.Find(_guestRoot).transform.position;
            maxSize = (hostPos - guestPos).magnitude * _maxSize;
        }else{
            Debug.LogError("ゲストとホストの位置が取得できません！");
            maxSize = 100;
        }

        _lightColor.a = _endAlpha;
        Sequence seq = DOTween.Sequence();
        seq.SetEase(Ease.OutQuad);
        seq.Append(transform.DOScale(Vector3.one * maxSize, _duration));
        seq.Join(_material.DOColor(_lightColor, _duration));
        seq.OnComplete(() =>
        {
            Destroy(gameObject);
        });
    }

}
