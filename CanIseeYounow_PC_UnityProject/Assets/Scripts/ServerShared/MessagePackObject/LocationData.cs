public class LocationData
{
    public string PanoId { get; }
    public double Lat { get; }
    public double Lng { get; }

    public LocationData(string panoId, double lat, double lng)
    {
        PanoId = panoId;
        Lat = lat;
        Lng = lng;
    }
}
