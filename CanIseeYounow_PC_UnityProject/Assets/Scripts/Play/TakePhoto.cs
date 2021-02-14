using UnityEngine;
using Cysharp.Threading.Tasks;

public class TakePhoto
{
    public static async UniTask<Texture2D> TakePhotoTex2D(GameObject cameraObj, Texture tex)
    {
        cameraObj.SetActive(true);
        //レンダリングを待つ
        await UniTask.Yield(PlayerLoopTiming.Update);
        //自動的にtextureファイルが上書きされる。
        cameraObj.SetActive(false);
        return ToTexture2D(tex);
    }
    static Texture2D ToTexture2D(Texture tex)
    {
        var sw = tex.width;
        var sh = tex.height;
        var format = TextureFormat.RGBA32;
        var result = new Texture2D(sw, sh, format, false);
        var currentRT = RenderTexture.active;
        var rt = new RenderTexture(sw, sh, 32);
        Graphics.Blit(tex, rt);
        RenderTexture.active = rt;
        var source = new Rect(0, 0, rt.width, rt.height);
        result.ReadPixels(source, 0, 0);
        result.Apply();
        result.SetPixels(result.GetPixels());
        result.Apply();
        RenderTexture.active = currentRT;
        return result;
    }
}

public static class RenderTextureExtension
{
    public static Texture2D toTexture2D(this RenderTexture rTex)
    {
        Texture2D tex = new Texture2D(512, 512, TextureFormat.RGB24, false);
        RenderTexture.active = rTex;
        tex.ReadPixels(new Rect(0, 0, rTex.width, rTex.height), 0, 0);
        tex.Apply();
        return tex;
    }
}
