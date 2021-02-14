using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// LocationDataとTexture(未実装)をキャッシュする
/// </summary>
public class MovementCasher
{
    List<LocationData> _locationDataList = new List<LocationData>();
    //配列をプロパティ的に使うために使っているインタフェース
    //=> はget onlyのプロパティ{get => 1行で済む処理}
    public IReadOnlyList<LocationData> LocationDataList => _locationDataList;
    public LocationData CurrentLocationData => LocationDataList.Count > 0 ? LocationDataList[0] : default;
    List<(CubemapFace, Texture2D)[]> _textureCashList = new List<(CubemapFace, Texture2D)[]>();
    public IReadOnlyList<(CubemapFace, Texture2D)[]> TextureCashList { get => _textureCashList; }
    private int _maxTexCashNumber = 10;

    public void CashLocationData(LocationData locationData)
    {
        _locationDataList.Insert(0, locationData);
    }

    public void CashTextures((CubemapFace, Texture2D)[] texData)
    {
        _textureCashList.Insert(0, texData);
        if (_textureCashList.Count > _maxTexCashNumber)
        {
            //texキャッシュの数制限
            _textureCashList.RemoveAt(_textureCashList.Count - 1);
        }
    }

    /// <summary>
    /// 最新のDataを一つ消して、一つ前のDataを取り出す
    /// </summary>
    /// <param name="locationData"></param>
    /// <returns></returns>
    //outが書いてあることで、LocationDataが構造体になっても、これに変更が出ない。
    //引数設定は、値を直接参照させずに迂回させ、別処理も走らせるため
    public bool PopBeforeLocationData(out LocationData locationData)
    {
        if (_locationDataList.Count < 2)
        {
            locationData = default;
            return false;
        }
        _locationDataList.RemoveAt(0);
        locationData = _locationDataList[0];
        return true;
    }
    public bool GetBeforeTexture(out (CubemapFace, Texture2D)[] texData)
    {
        if (_textureCashList.Count < 2)
        {
            texData = default;
            return false;
        }
        _textureCashList.RemoveAt(0);
        texData = _textureCashList[0];
        return true;
    }
}