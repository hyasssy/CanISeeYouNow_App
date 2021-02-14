using UnityEngine;

public class IsEventSwitch : MonoBehaviour
{
    [SerializeField]
    GameObject[] _noramalGroup, _showGroup;
    void Awake()
    {
        var eventType = Resources.Load<DebugParameter>("DebugParameter").EventType;
        switch(eventType){
            case 0:
                foreach(GameObject g in _showGroup) Destroy(g);
                foreach(GameObject g in _noramalGroup) g.SetActive(true);
                break;
            case 1:
                foreach(GameObject g in _noramalGroup) Destroy(g);
                foreach(GameObject g in _showGroup) g.SetActive(true);
                break;
            default: break;
        }
    }

}
