using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

[Serializable]
public class PassJson
{
    public string password;
}
public static class CheckPassword
{
    const string DATAURL = "https://hyasssy.github.io/CanISeeYouNow_App/data/cisyn_data.json";
    public static async UniTask<bool> CollatePassWord(string inputPass, Action<float> progress)
    {
        using (var uwr = UnityWebRequest.Get(DATAURL))
        {
            var request = uwr.SendWebRequest();
            double count = 0;
            while (true)
            {
                if (uwr.isHttpError || uwr.isNetworkError)
                {
                    //エラー
                    throw new Exception(uwr.error);
                }

                if (request.isDone)
                {
                    //正常終了
                    Debug.Log("Networking Success");
                    // var json = JsonUtility.FromJson<PassJson>(uwr.downloadHandler.text);//変換に失敗した場合はSystem.ArgumentExceptionが飛ぶ
                    var json = JsonNode.Parse(uwr.downloadHandler.text);
                    var password = json["password"].Get<string>();
                    var result = CryptExpansion.Decrypt(password) == inputPass;
                    Debug.Log("input=" + inputPass + ", result=" + result);
                    if (result)
                    { //when put correct pass, do this process
                        DebugParameter debugParameter = Resources.Load<DebugParameter>("DebugParameter");
                        for (int i = 0; i < json["apikey"].Count; i++)
                        { //同じものがまだ追加されていなければ、追加。
                            bool b = true;
                            string key = json["apikey"][i].Get<string>();
                            key = CryptExpansion.Decrypt(key);
                            foreach (string s in debugParameter.APIKeys)
                            {
                                if (s == key) b = false;
                            }
                            if (b)
                            {
                                debugParameter.APIKeys.Add(key);
                            }
                        }
                    }
                    return result;
                }
                await UniTask.Yield();
                Debug.Log("ダウンロード済みバイト数=" + uwr.downloadedBytes + "所用時間=" + count);
                count += Time.deltaTime;
                progress(request.progress);
            }
        }
    }

}