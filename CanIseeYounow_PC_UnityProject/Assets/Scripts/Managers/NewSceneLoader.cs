using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UniRx;
using Cysharp.Threading.Tasks;

public enum NewScene
{
    p0_Background,
    p1_Matching,
    p2_AvatarSelect,
    p3_LocationInput,
    p4_Wait,
    p5_Play,
    p6_End,
}

public class NewSceneLoader : MonoBehaviour
{
    public NewScene MainScene { get; private set; } = NewScene.p0_Background;
    public NewScene ChildrenScene { get; private set; } = NewScene.p1_Matching;
    Subject<NewScene> _onLoad = new Subject<NewScene>();
    public IObservable<NewScene> OnLoad => _onLoad;
    async UniTask LoadSceneSingle(NewScene scene)
    {
        await SceneManager.LoadSceneAsync(scene.ToString(), LoadSceneMode.Single);
        MainScene = scene;
        _onLoad.OnNext(scene);
        Debug.Log($"=================={scene}=========================");
    }
    async UniTask LoadSceneAdditive(NewScene scene)
    {
        await SceneManager.LoadSceneAsync(scene.ToString(), LoadSceneMode.Additive);
        ChildrenScene = scene;
        _onLoad.OnNext(scene);
        Debug.Log($"=================={scene}=========================");
    }

    async UniTask UnloadAdditiveScenes()
    {
        await SceneManager.UnloadSceneAsync(ChildrenScene.ToString());
    }

    async UniTask LoadSetup()
    {
        if (MainScene == NewScene.p0_Background)
        {
            //await UniTask.Delay(TimeSpan.FromSeconds(0.5f));
            await UnloadAdditiveScenes();
        }
        else
        {
            await LoadSceneSingle(NewScene.p0_Background);
        }
    }

    void Start()
    {
        DontDestroyOnLoad(gameObject);
        if (!Resources.Load<DebugParameter>("DebugParameter").PlayLocal)
            LoadSceneAdditive(NewScene.p1_Matching).Forget();
    }

    public async UniTask LoadMatching()
    {
        await UniTask.WhenAll(FindObjectOfType<HandlePPVolume>().UpdateLensScale(NewScene.p1_Matching),
        FindObjectOfType<FadeComponent>().FadeOutScaleUp());
        LoadSetup().Forget();
    }
    public async UniTask LoadAvatarSelect()
    {
        await UniTask.WhenAll(FindObjectOfType<HandlePPVolume>().UpdateLensScale(NewScene.p2_AvatarSelect),
        FindObjectOfType<FadeComponent>().FadeOutScaleUp());
        LoadSetup().Forget();
        await LoadSceneAdditive(NewScene.p2_AvatarSelect);
    }
    public async UniTask LoadLocationInput()
    {
        await UniTask.WhenAll(FindObjectOfType<HandlePPVolume>().UpdateLensScale(NewScene.p3_LocationInput),
        FindObjectOfType<FadeComponent>().FadeOutScaleUp());
        LoadSetup().Forget();
        await LoadSceneAdditive(NewScene.p3_LocationInput);
    }
    public async UniTask LoadWait()
    {
        await UniTask.WhenAll(FindObjectOfType<HandlePPVolume>().UpdateLensScale(NewScene.p4_Wait),
        FindObjectOfType<FadeComponent>().FadeOutScaleUp());
        LoadSetup().Forget();
        await LoadSceneAdditive(NewScene.p4_Wait);
    }
    public async UniTask LoadPlay()
    {
        await UniTask.WhenAll(FindObjectOfType<HandlePPVolume>().UpdateLensScale(NewScene.p5_Play),
        FindObjectOfType<FadeComponent>().FadeOutScaleUp(),
        FindObjectOfType<WhiteOutFade>().WhiteOut());
        await LoadSceneSingle(NewScene.p5_Play);
    }
    public async UniTask LoadEnd()
    {
        Debug.Log("LoadEnd");
        await LoadSceneSingle(NewScene.p6_End);
    }
}
