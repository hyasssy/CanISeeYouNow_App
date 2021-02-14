using UniRx;

public interface ILocationPusher
{
    IReadOnlyReactiveProperty<LocationData> LocationData { get; }
}
