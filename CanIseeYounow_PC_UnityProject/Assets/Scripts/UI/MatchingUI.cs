using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class MatchingUI : MonoBehaviour
{
    [SerializeField]
    Text _keywordText, _isMatchText;
    [SerializeField]
    Color _hostColor, _guestColor;
    const string ALONE = "Alone...", MATCHED = "Matched";

    void Start()
    {
        var matchingManager = FindObjectOfType<PhotonMatchingManager>();
        _keywordText.color = matchingManager.IsHost ? _hostColor : _guestColor;//hostguestで色変える。
        _isMatchText.color = matchingManager.IsHost ? _hostColor : _guestColor;
        _keywordText.text = $"Room \"{matchingManager.Room.Name}\"";
        _isMatchText.text = matchingManager.Room.PlayerCount == 2 ? MATCHED : ALONE;
        matchingManager.OnEnterRoom
            .Subscribe(_ => _isMatchText.text = MATCHED)
            .AddTo(this);
    }
}
