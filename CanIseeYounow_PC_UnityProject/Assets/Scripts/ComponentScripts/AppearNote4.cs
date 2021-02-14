using UnityEngine;
using Cysharp.Threading.Tasks;
using UniRx;
using UniRx.Triggers;

public class AppearNote4 : MonoBehaviour
{
    //Note4を出現させる。Note4が消える時、ベル出現。
    [SerializeField]
    float delay = 480f;
    [SerializeField]
    GameObject note4;
    async private void Start()
    {
        await UniTask.Delay((int)(delay * 1000));
        this.UpdateAsObservable()
        .First(_ => Input.GetKeyDown(KeyCode.G))
        .Subscribe(_ =>
        {
            if(!FindObjectOfType<SessionManager>().IsMeeted){//きもいけど、会った後は出ないようにする。
                note4.SetActive(true);
            }
        });
    }
}