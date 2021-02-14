using UnityEngine;

public class VoiceChatSwither : MonoBehaviour
{
    DebugParameter _debugParameter;
    private void Start() {
        _debugParameter = Resources.Load<DebugParameter>("DebugParameter");
    }
    public void Switch(bool b){
        _debugParameter.VoiceChatSwitch(b);
        Debug.Log("Voics Chat : " + _debugParameter.IsUseVoiceChat);
    }
}
