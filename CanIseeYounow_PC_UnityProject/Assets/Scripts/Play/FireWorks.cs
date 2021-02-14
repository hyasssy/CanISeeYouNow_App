using UnityEngine;
using Cysharp.Threading.Tasks;
using System;

public class FireWorks
{
    public static async UniTask Fire(GameObject prefab, float duration, Transform playerPos)
    {
        float startTime = Time.time;
        while (Time.time - startTime < duration)
        {
            GameObject.Instantiate(prefab, RandomFirePos(playerPos), Quaternion.identity);
            await UniTask.Delay(TimeSpan.FromSeconds((double)UnityEngine.Random.Range(0.1f, 3f)), false, PlayerLoopTiming.Update);
        }
    }
    static Vector3 RandomFirePos(Transform playerPos)
    {
        Vector3 randomPos = new Vector3(UnityEngine.Random.Range(-10f, 10f), UnityEngine.Random.Range(10f, 15f), UnityEngine.Random.Range(-10f, 10f));
        return playerPos.position + randomPos;
    }
}
