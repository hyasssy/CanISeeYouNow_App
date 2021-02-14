using System;
using UnityEngine;
using UniRx;
using System.Linq;
using System.Collections.Generic;

public class UpdateManager : MonoBehaviour
{
    public IObservable<long> InputUpdate;
    public IObservable<long> NormalUpdate;
    public IObservable<long> FixedUpdate;
    public IObservable<long> PostFixedUpdate;

    public readonly BoolReactiveProperty paused = new BoolReactiveProperty(false);

    public void Awake()
    {
        paused.Subscribe(x => Time.timeScale = x ? 0 : 1);
        InputUpdate = Observable.EveryUpdate().Where(_ => !paused.Value).Share();
        NormalUpdate = Observable.EveryUpdate().Where(_ => !paused.Value).Share();
        FixedUpdate = Observable.EveryFixedUpdate().Where(_ => !paused.Value).Share();
        PostFixedUpdate = Observable.EveryFixedUpdate().Where(_ => !paused.Value).Share();
    }
}