using UnityEngine;
using UnityEngine.AddressableAssets;
using Cysharp.Threading.Tasks;

public static class AvatarExtension
{
    public static async UniTask<GameObject> InstantiateAsync(this Avatar avatar, Transform parent)
    {
        Debug.Log($"Instantiate {avatar}");
        return await Addressables.InstantiateAsync($"Avatars/{avatar}.prefab", parent);
    }
}
