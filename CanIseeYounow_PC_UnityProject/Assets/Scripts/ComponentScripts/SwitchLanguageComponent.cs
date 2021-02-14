using UnityEngine;

///<summary>
///このコンポーネントを持ったオブジェがアクティブになった時、指定したテキストの言語をセットする。言語が変わるテキストは都度親グループを作り、そこにこのコンポーネントをアタッチ
///</summary>
public class SwitchLanguageComponent : MonoBehaviour
{
    [SerializeField]
    GameObject[] _japObjs, _engObjs;
    private void OnEnable()
    {
        var languageType = Resources.Load<DebugParameter>("DebugParameter").LanguageType;
        foreach (GameObject obj in _japObjs) obj.SetActive(languageType == Language.Japanese);
        foreach (GameObject obj in _engObjs) obj.SetActive(languageType == Language.English);
    }
}
