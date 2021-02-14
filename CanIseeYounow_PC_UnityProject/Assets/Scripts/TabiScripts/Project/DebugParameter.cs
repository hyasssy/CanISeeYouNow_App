using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "PlayLocalFlag")]
public class DebugParameter : ScriptableObject
{
    [NonEditable]
    public List<string> APIKeys;//ここに暗号化を解いて入れる。
    public bool CanUse => APIKeys.Count > 0;
    public void RemoveKey (string key) {
        APIKeys.Remove (key);
    }

    public string APIkey {
        get {
            if (!CanUse) return null;
            int i = Random.Range (0, APIKeys.Count);
            return APIKeys[i];
        }
    }



    [field: SerializeField, RenameField(nameof(PlayLocal))]
    public bool PlayLocal { get; private set; } = false;

    [field: SerializeField, RenameField(nameof(Debug))]
    public bool Debug { get; private set; } = false;

    [field: SerializeField, RenameField(nameof(FirstLocationReach))]
    public int FirstLocationReach = 225;
    [field: SerializeField, RenameField(nameof(EventType))]//0=Normal, 1=EventIn1Room
    public int EventType { get; private set; } = 0;
    [field: SerializeField, NonEditable]
    public bool IsUseVoiceChat { get; private set; } = false;
    [field: SerializeField, NonEditable]

    public Language LanguageType { get; private set; } = Language.Japanese;//0:Ja,1:En
    public void VoiceChatSwitch(bool b){
        IsUseVoiceChat = b;
    }
    public void LanguageSwitch(Language l){
        LanguageType = l;
    }
}
public enum Language
    {
        Japanese,
        English
    }
