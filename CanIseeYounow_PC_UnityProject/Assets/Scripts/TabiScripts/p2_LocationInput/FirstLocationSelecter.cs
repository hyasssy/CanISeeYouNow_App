using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;
using System.Text.RegularExpressions;

public class FirstLocationSelecter : MonoBehaviour
{
    [SerializeField] InputField _latLngInput;
    [SerializeField] Button[] _buttons;
    [SerializeField] GameObject _errorPanel;

    void Start()
    {
        var holder = FindObjectOfType<FirstLocationHolder>();
        var sceneLoader = FindObjectOfType<NewSceneLoader>();
        foreach (Button b in _buttons)
        {
            b.OnPointerClickAsObservable()
                .Subscribe(async _ =>
                {
                    try
                    {
                        MatchCollection latLngText = Regex.Matches(_latLngInput.text, @"-?\d+[.]\d+");
                        if (latLngText.Count != 2)
                        {
                        //ここでクリップボードをinputfieldに入れてみる。その上でまたダメだったら。
                        MatchCollection clipboard = Regex.Matches(GUIUtility.systemCopyBuffer, @"-?\d+[.]\d+");
                            if (clipboard.Count == 2)
                            {
                                _latLngInput.text = GUIUtility.systemCopyBuffer;
                                latLngText = clipboard;
                            }
                            else
                            {
                                Debug.Log("緯度経度の数列が見当たりません。");
                                return;
                            }
                        }
                        var lat = double.Parse(latLngText[0].Value);
                        var lng = double.Parse(latLngText[1].Value);
                        var locationData = await StreetViewAPI.GetLocationData(lat, lng);
                        holder.SetFirstLocation(locationData);
                        Debug.Log("Send Host's LocationData");
                        if (b.interactable == false) return;
                        b.interactable = false;
                        await sceneLoader.LoadWait();

                    }
                    catch
                    {
                        _errorPanel?.SetActive(true);
                    }
                });
        }
        this.UpdateAsObservable()
            .Where(_ => Input.GetKeyDown(KeyCode.Return))
            .Subscribe(async _ =>
            {
                try
                {
                    MatchCollection latLngText = Regex.Matches(_latLngInput.text, @"-?\d+[.]\d+");
                    if(latLngText.Count != 2){
                        //ここでクリップボードをinputfieldに入れてみる。その上でまたダメだったら。
                        MatchCollection clipboard = Regex.Matches(GUIUtility.systemCopyBuffer, @"-?\d+[.]\d+");
                        if(clipboard.Count == 2){
                            _latLngInput.text = GUIUtility.systemCopyBuffer;
                            latLngText = clipboard;
                        }
                        else
                        {
                            Debug.Log("緯度経度の数列が見当たりません。");
                            return;
                        }
                    }
                    var lat = double.Parse(latLngText[0].Value);
                    var lng = double.Parse(latLngText[1].Value);
                    var locationData = await StreetViewAPI.GetLocationData(lat, lng);
                    holder.SetFirstLocation(locationData);
                    Debug.Log("Send Host LocationData");
                    if(_buttons[0].interactable == false) return;
                    foreach(Button b in _buttons) b.interactable = false;
                    await sceneLoader.LoadWait();

                }
                catch
                {
                    _errorPanel?.SetActive(true);
                }
            });
    }
}