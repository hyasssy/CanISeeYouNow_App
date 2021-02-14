using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using DG.Tweening;
using Cysharp.Threading.Tasks;

public class HandlePPVolume : MonoBehaviour
{
    LensDistortion _lensDistortion;

    float _currentScale;

    [SerializeField]
    Ease _ease = Ease.InCubic;

    [SerializeField]
    float _p0Scale = 0.4f;
    [SerializeField]
    float _p1Scale = 0.8f;
    [SerializeField]
    float _p2Scale = 1.2f;
    [SerializeField]
    float _p3Scale = 2f;
    [SerializeField]
    float _p4Scale = 3f;
    [SerializeField]
    float _duration = 1.3f;


    void Start()
    {
        var volume = GetComponent<PostProcessVolume>();
        _lensDistortion = volume.profile.GetSetting<LensDistortion>();
        _currentScale = _lensDistortion.scale;
    }

    public async UniTask UpdateLensScale(NewScene scene)
    {
        var nextScale = _currentScale;
        var matchingManager = FindObjectOfType<PhotonMatchingManager>();
        switch (scene)
        {
            case NewScene.p1_Matching:
                nextScale = _p0Scale;
                break;
            case NewScene.p2_AvatarSelect:
                nextScale = _p1Scale;
                break;
            case NewScene.p3_LocationInput:
                nextScale = _p2Scale;
                break;
            case NewScene.p4_Wait:
                if (matchingManager.IsHost)
                    nextScale = _p3Scale;
                else
                    nextScale = _p2Scale;
                break;
        }
        await UpdateLensScale(nextScale, _duration);
    }

    public async UniTask UpdateLensScale(float nextScale, float time)
    {
        await DOTween.To(
        () => _currentScale,
        x =>
        {
            _currentScale = x;
            _lensDistortion.scale.value = _currentScale;
        },
        nextScale,
        time)
        .SetEase(_ease)
        .AsyncWaitForCompletion();
    }
}
