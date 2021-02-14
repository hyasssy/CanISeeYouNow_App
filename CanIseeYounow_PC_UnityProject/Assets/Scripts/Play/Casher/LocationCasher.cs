using System;
using System.Collections.Generic;

public class LocationCasher
{
    List<LocationData> _locationList = new List<LocationData>();
    int _head = 0;

    public LocationData CurrentLocation => _locationList[_head];

    public LocationCasher(LocationData locationData)
    {
        _locationList.Add(locationData);
    }

    public LocationData AddCash(LocationData locationData)
    {
        _head++;
        _locationList.RemoveRange(_head, _locationList.Count - _head);
        _locationList.Add(locationData);
        return locationData;
    }

    public LocationData MoveBack()
    {
        _head = Math.Max(_head - 1, 0);
        return _locationList[_head];
    }
}
