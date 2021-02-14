using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class MouseOnButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    Transform _avatarRoot;

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        _avatarRoot.DOScale(Vector3.one * 1.2f, 0.2f);
    }
    public void OnPointerExit(PointerEventData pointerEventData)
    {
        _avatarRoot.DOScale(Vector3.one, 0.2f);
    }
}
