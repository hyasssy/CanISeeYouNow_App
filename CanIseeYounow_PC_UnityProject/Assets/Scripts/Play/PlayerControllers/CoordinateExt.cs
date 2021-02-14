using UnityEngine;

public static class CoordinateExt
{
    ///<summary>
    /// 緯度によって、距離あたりの経度を座標変換していく係数が変わってくるその係数計算。地球の扁平率は無視。
    /// 経度にこの係数をかけて補正する。
    ///</summary>
    public static double LngCoef(double l_tmpLat)
    {
        float latValue = Mathf.Abs((float)l_tmpLat);
        double value = Mathf.Cos(latValue);
        return value;
    }

    public static (double lat, double lng) FormatCoordinate(double lat, double lng)
    {
        var newLat = lat;
        var newLng = lng;
        if (lat >= 90)
        {
            newLat = 180 - lat;
        }
        else if (lat <= -90)
        {
            newLat = -180 - lat;
        }

        if (lng > 180)
        {
            newLng = lng - 360;
        }
        else if (lng < -180)
        {
            newLng = lng + 360;
        }
        return (newLat, newLng);
    }

    public static Vector3 ToVct3Position(LocationData data, LocationData original)
    {
        var lat = data.Lat - original.Lat;
        var lng = data.Lng - original.Lng;
        var z = lat / 0.000045;
        int isEastHemis = data.Lng < 0 ? 1 : -1;
        var x = lng / 0.000045 / CoordinateExt.LngCoef(data.Lat) * isEastHemis;
        return new Vector3((float)x, 0, (float)z);
    }

    ///<summary>
    ///クリックした向きのdoubleのVector2dを返します。(lat, lng)
    ///</summary>
    public static (double lat, double lng) GetLatLng(LocationData locationData, int reach, Vector3 clickVector)
    {
        //get dir for the clicked point normalized on x-z plane.
        Vector3 dir = clickVector * reach;
        //経度によって挙動が逆になるのを修正
        int isEastHemis = locationData.Lng < 0 ? 1 : -1;
        //1m = 0.000045 degree of longitude
        var (lat, lng) = (dir.z * 0.000045, isEastHemis * dir.x * 0.000045 * CoordinateExt.LngCoef(locationData.Lat));
        return CoordinateExt.FormatCoordinate(lat + locationData.Lat, lng + locationData.Lng);
    }
}
