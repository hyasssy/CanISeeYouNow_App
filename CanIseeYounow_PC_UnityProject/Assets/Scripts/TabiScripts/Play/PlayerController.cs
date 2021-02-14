using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;
using System;
using System.Linq;

public class PlayerController : MonoBehaviour
{
    [SerializeField] bool _isHost;
    [SerializeField] float _verRotateSpeed = 4f, _horRotateSpeed = 4f;
    bool _isMain;
    IFirstLocationHolder _locationHolder;
    InputHandler _inputHandler;
    ErrorPanelHandler _errorPanelHandler;
    [SerializeField]
    Transform _avatarRoot;//アバターのx回転をなくす。

    async void Start()
    {
        var locationHolder = FindObjectOfType<FirstLocationHolder>();
        var inputHandler = FindObjectOfType<InputHandler>();
        var matchingManager = FindObjectOfType<PhotonMatchingManager>();
        var _playLocalFlag = Resources.Load<DebugParameter>("DebugParameter");
        _inputHandler = inputHandler;
        _locationHolder = locationHolder;
        _isMain = matchingManager.IsHost == _isHost;
        if (_isMain)
        {
            var originalPoint = _locationHolder.HostLocation;
            transform.position = CoordinateExt.ToVct3Position(_locationHolder.MainPlayerLocation, originalPoint);
            var cubemap = GameObject.Find("Cubemap").transform;
            _errorPanelHandler = FindObjectOfType<ErrorPanelHandler>();

            inputHandler.LocationData
                .Where(_ => !_playLocalFlag.PlayLocal)
                .Where(x => x != null)
                .Select(x => CoordinateExt.ToVct3Position(x, originalPoint))
                .Subscribe(async x =>
                {
                    var pos = x + Vector3.forward * 0.35f * (_isHost ? 1 : -1);
                    Time.timeScale = 0;
                    transform.position = cubemap.position = pos;
                    await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate);
                    Time.timeScale = 1;
                });
        }

        var avatarHolder = FindObjectOfType<NewAvatarHolder>();
        var avatar = _isHost ? avatarHolder.HostAvatar : avatarHolder.GuestAvatar;
        await avatar.InstantiateAsync(transform.Find("Parent/AvatarRoot/AvatarParent"));
    }

    public bool _canRotate = true;

    void Update()
    {
        if (_isMain && _canRotate)
        {
            Rotate();
        }
    }

    void Rotate()
    {
        if (!Input.GetMouseButton(0)) return;
        var X_Rotation = Input.GetAxis("Mouse X") * _verRotateSpeed; // * SetGeer();
        var Y_Rotation = Input.GetAxis("Mouse Y") * _horRotateSpeed; // * SetGeer();
        transform.Rotate(0, X_Rotation, 0);
        transform.GetChild(0).Rotate(-Y_Rotation, 0, 0);
        _avatarRoot.Rotate(Y_Rotation, 0, 0);
    }

    [SerializeField] float _duration = 1.5f; //何秒でむき終わるか

    // [SerializeField]
    float _delay = 5f;
    float _targetFieldOfView = 30f;

    public async UniTask ZoomIn()
    {
        if (!_isMain) return;
        var targetName = $"{(_isHost ? "Guest" : "Host")}PlayerRoot(Clone)";
        var target = GameObject.Find(targetName).transform.position - Vector3.up * -0f;
        _canRotate = false;
        var cameraParent = transform.Find("Parent/Cameras");
        var cameras = GetComponentsInChildren<Camera>().ToList();

        Camera main = Camera.main;
        float p = 0;
        float fieldOfView = main.fieldOfView;
        float primaryFieldOfView = main.fieldOfView;
        float time = 0;
        Quaternion primaryRot = cameraParent.rotation;
        Quaternion targetRot = Quaternion.LookRotation(target - cameraParent.position, Vector3.up);
        while (time < _duration)
        {
            cameras.ForEach(camera =>
            {
                camera.transform.rotation = Quaternion.Lerp(primaryRot, targetRot, p);
                camera.fieldOfView = Mathf.Lerp(primaryFieldOfView, _targetFieldOfView, p);
            });
            time += Time.deltaTime;
            p = Easing.QuadInOut(time, _duration, 0, 1);
            await UniTask.Yield();
        }

        await UniTask.Delay(TimeSpan.FromSeconds(_delay));
        time = 0;
        while (time < _duration)
        {
            cameras.ForEach(camera =>
            {
                camera.transform.rotation = Quaternion.Lerp(targetRot, primaryRot, p);
                camera.fieldOfView = Mathf.Lerp(_targetFieldOfView, primaryFieldOfView, p);
            });
            time += Time.deltaTime;
            p = Easing.QuadInOut(time, _duration, 0, 1);
            await UniTask.Yield();
        }

        _canRotate = true;
    }
}