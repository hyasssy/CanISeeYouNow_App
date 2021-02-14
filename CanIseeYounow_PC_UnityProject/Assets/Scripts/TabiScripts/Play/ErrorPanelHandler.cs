using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class ErrorPanelHandler : MonoBehaviour
{
    [SerializeField] GameObject _errorPanel = default;
    [SerializeField] Transform _mainPlayerObjects = default;
    [SerializeField] GameObject _noMoneyPanel = default;
    GameObject _panelInstance;
    Camera _mainCamera;

    void Start()
    {
        var inputHandler = FindObjectOfType<InputHandler>();
        _mainCamera = Camera.main;
        StreetViewAPI.StatusStream
            .Subscribe(x =>
            {
                if (x == MetaDataStatus.NoData)
                {
                }
                else if (x == MetaDataStatus.NoMoney)
                {
                    _noMoneyPanel.SetActive(true);
                }
                else if (x == MetaDataStatus.OK)
                {
                    _noMoneyPanel.SetActive(false);
                    if (_panelInstance != null)
                    {
                        Destroy(_panelInstance);
                        _panelInstance = null;
                    }
                }
            }).AddTo(this);
    }

    public void DisplayNodataPanel()
    {
        var direction = LocationGetter.GetClickVector(_mainCamera);
        direction = new Vector3(direction.x, 0, direction.z) * 1.2f;
        var angle = Mathf.Atan2(direction.z, direction.x) * -180 / Mathf.PI + 90f;
        if (_panelInstance != null)
            Destroy(_panelInstance);
        _panelInstance = Instantiate(_errorPanel, _mainPlayerObjects.position + direction,
            Quaternion.Euler(new Vector3(0, angle, 0)), transform);
    }

    [SerializeField] GameObject _distanceLimitPanel = default;

    public void DisplayDistanceLimitPanel()
    {
        var direction = LocationGetter.GetClickVector(_mainCamera);
        direction = new Vector3(direction.x, 0, direction.z) * 1.2f;
        var angle = Mathf.Atan2(direction.z, direction.x) * -180 / Mathf.PI + 90f;
        if (_panelInstance != null)
            Destroy(_panelInstance);
        _panelInstance = Instantiate(_distanceLimitPanel, _mainPlayerObjects.position + direction,
            Quaternion.Euler(new Vector3(0, angle, 0)), transform);
    }
}