using UnityEngine;

public class MockFirstLocationDataHolder : MonoBehaviour, IFirstLocationHolder
{
    [SerializeField]
    double _lat;
    [SerializeField]
    double _lng;
    [SerializeField]
    string _panoId;

    LocationData _hostLocation;
    public LocationData HostLocation
    {
        get
        {
            if (_hostLocation == null)
                _hostLocation = new LocationData(_panoId, _lat, _lng);
            return _hostLocation;
        }
    }

    public LocationData GuestLocation => throw new System.NotImplementedException();

    public LocationData MainPlayerLocation => HostLocation;
}
