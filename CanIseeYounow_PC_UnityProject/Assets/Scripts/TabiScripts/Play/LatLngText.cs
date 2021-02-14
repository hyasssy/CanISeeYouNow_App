using UnityEngine;
using UnityEngine.UI;
using UniRx;
using System;

public class LatLngText : MonoBehaviour
{
    void Start()
    {
        var inputHandler = FindObjectOfType<InputHandler>();
        var text = GetComponent<Text>();
        inputHandler.LocationData
            .Where(x => x != null)
            .Subscribe(x =>
            {
                var newLat = Math.Round(x.Lat, 5);
                var newLng = Math.Round(x.Lng, 5);
                text.text = $"{newLat}, {newLng}";
            });
    }
}
