using UnityEngine;
using DG.Tweening;

public class ChangeFOV : MonoBehaviour
{
    [SerializeField] private Camera[] cameras;
    [SerializeField] private float _primaryValue, targetZoomInAngle, duration;

    private void Start()
    {
        _primaryValue = 75f;
    }

    //ZoomIn->ZoomOutの想定。
    public void ZoomIn()
    {
        DOTween.To(() => _primaryValue,
                x =>
                {
                    foreach (var cam in cameras)
                        cam.fieldOfView = x;
                }, targetZoomInAngle, duration)
            .SetEase(Ease.InOutSine);
    }
    public void ZoomOut()
    {
        DOTween.To(() => targetZoomInAngle,
                x =>
                {
                    foreach (var cam in cameras)
                        cam.fieldOfView = x;
                }, _primaryValue, duration)
            .SetEase(Ease.InOutSine);
    }
}