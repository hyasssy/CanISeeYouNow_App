using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetParam : MonoBehaviour
{
    private void Awake() {
        DebugParameter debugParameter = Resources.Load<DebugParameter>("DebugParameter");

        debugParameter.APIKeys = new List<string>();
        debugParameter.VoiceChatSwitch(false);
        debugParameter.LanguageSwitch(Language.Japanese);
    }
}
