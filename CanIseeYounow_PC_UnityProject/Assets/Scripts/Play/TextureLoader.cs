using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

/// <summary>
/// TextureのLoadを行うstaticクラス
/// </summary>
public static class TextureLoader
{
    const int WIDTH = 512;
    const int HEIGHT = 512;
    const string SOURCE = "outdoor";// manager.onlyOutside? "outdoor" : "default";
    /// <summary>
    /// 指定した面のTextureを取得する
    /// </summary>
    /// <param name="panoId"></param>
    /// <param name="cubemapFace"></param>
    /// <returns></returns>
    public static async UniTask<(CubemapFace face, Texture2D texture)> LoadTexture(string panoId, CubemapFace cubemapFace)
    {
        var (heading, pitch) = GetHeading_Pitch(cubemapFace);
        var texture = await StreetViewAPI.GetTexture(WIDTH, HEIGHT, panoId, heading, pitch, SOURCE);
        return (cubemapFace, texture);
    }

    /// <summary>
    /// 六面のTextireをまとめて取得する
    /// </summary>
    /// <param name="panoId"></param>
    /// <returns></returns>
    public static UniTask<(CubemapFace, Texture2D)[]> LoadHexahedronTexture(string panoId)
    {
        // WheAllは引数に取ったTaskたちが全員終わるまで待つ関数
        return UniTask.WhenAll(YieldTask());

        IEnumerable<UniTask<(CubemapFace face, Texture2D texture)>> YieldTask()
        {
            yield return LoadTexture(panoId, CubemapFace.PositiveX);
            yield return LoadTexture(panoId, CubemapFace.NegativeX);
            yield return LoadTexture(panoId, CubemapFace.PositiveY);
            yield return LoadTexture(panoId, CubemapFace.NegativeY);
            yield return LoadTexture(panoId, CubemapFace.PositiveZ);
            yield return LoadTexture(panoId, CubemapFace.NegativeZ);
        }
    }

    /// <summary>
    /// CubeMapFaceからheadingとpitchを割り出す
    /// </summary>
    /// <param name="cubemapFace">六面体の面</param>
    /// <returns>headingとpitchのタプル型</returns>
    static (double heading, double pitch) GetHeading_Pitch(CubemapFace cubemapFace)
    {
        switch (cubemapFace)
        {
            case CubemapFace.NegativeZ:
                return (0, 0);
            case CubemapFace.NegativeY:
                return (180, 90);
            case CubemapFace.PositiveY:
                return (180, -90);
            case CubemapFace.NegativeX:
                return (90, 0);
            case CubemapFace.PositiveZ:
                return (180, 0);
            case CubemapFace.PositiveX:
                return (270, 0);
            default:
                return (0, 0);
        }
    }
}
