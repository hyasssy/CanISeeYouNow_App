using System;
using UnityEngine;

[Serializable]
public class MetaDataJson : ISerializationCallbackReceiver
{
    [SerializeField]
    [HideInInspector]
    string copyright;
    public string Copyright => copyright;

    [SerializeField]
    [HideInInspector]
    string date;
    public string Date => date;

    [SerializeField]
    [HideInInspector]
    Location location;
    public Location Location => location;

    [SerializeField]
    [HideInInspector]
    string pano_id;
    public string PanoId => pano_id;

    [SerializeField]
    [HideInInspector]
    string status;
    public string Status => status;

    public LocationData LocationData { get; private set; }

    void ISerializationCallbackReceiver.OnBeforeSerialize()
    {
    }

    void ISerializationCallbackReceiver.OnAfterDeserialize()
    {
        LocationData = new LocationData(PanoId, location.lat, location.lng);
    }
}

[Serializable]
public class Location
{
    public double lat;
    public double lng;
}