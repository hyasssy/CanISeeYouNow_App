using UnityEngine;
using UniRx;
using System.Linq;
using System.Collections.Generic;
using UniRx.Triggers;

public class LayerSwitcher : MonoBehaviour
{
    [SerializeField]
    Collider _collider = default;
    void Start()
    {
        IEnumerable<GameObject> renderers = null;
        void SwitchiLayer(string layerName)
        {
            if (renderers == null)
            {
                renderers = gameObject.GetComponentsInChildren<MeshRenderer>().Select(x => x.gameObject);
                renderers = gameObject.GetComponentsInChildren<SkinnedMeshRenderer>().Select(x => x.gameObject).Concat(renderers);
            }
            foreach (var x in renderers)
                x.layer = LayerMask.NameToLayer(layerName);
        }

        var sessionManager = FindObjectOfType<SessionManager>();

        _collider.OnTriggerEnterAsObservable()
            .Where(_ => !sessionManager.Meeted.Value)
            .Where(x => x.name == "CoreCollider" || x.name == "LayerSwitchCollider")
            .Do(_ => Debug.Log("Switchi to IgnoreDepth"))
            .Subscribe(_ => SwitchiLayer("IgnoreDepth"));
        _collider.OnTriggerExitAsObservable()
            .Where(_ => !sessionManager.Meeted.Value)
            .Where(x => x.name == "CoreCollider" || x.name == "LayerSwitchCollider")
            .Do(_ => Debug.Log("Switchi to Default"))
            .Subscribe(_ => SwitchiLayer("Default"));

        sessionManager.Meeted
            .Where(x => x)
            .Do(_ => Debug.Log("Switchi to IgnoreDepth"))
            .Subscribe(_ => SwitchiLayer("IgnoreDepth"));
    }
}
