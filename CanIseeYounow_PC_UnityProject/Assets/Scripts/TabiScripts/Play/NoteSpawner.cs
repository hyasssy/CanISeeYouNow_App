using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using Photon.Voice.PUN;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;

public class NoteSpawner : MonoBehaviour
{
    [Serializable]
    public class ColliderData
    {
        [field: SerializeField, RenameField(nameof(Collider))]
        public Collider Collider { get; private set; }

        [field: SerializeField, RenameField(nameof(Range))]
        public int Range { get; private set; }

        [field: SerializeField, RenameField(nameof(AudioClip))]
        public AudioClip AudioClip { get; private set; }
    }
    [SerializeField]
    List<ColliderData> _colliderData = new List<ColliderData>();
    [SerializeField]
    List<GameObject> _notes = new List<GameObject>();
    [SerializeField]
    AudioSource _audioSource;
    Transform _canvas;
    Camera[] _cameras;

    void Start()
    {
        _canvas = GameObject.Find("GUI/Canvas").transform;
        _cameras = FindObjectsOfType<Camera>();
        foreach (var item in _colliderData)
        {
            item.Collider.OnTriggerEnterAsObservable()
                .Do(xxx => Debug.Log(xxx))
                .Subscribe(_ =>
                {
                    _audioSource.PlayOneShot(item.AudioClip);//距離の読み上げ
                    var range = item.Range;
                    Destroy(item.Collider.gameObject);
                    // if (!(range == 100 || range == 200)) return;
                    // PhotonMatchingManager pmm = FindObjectOfType<PhotonMatchingManager>();
                    // if(!pmm.IsHost) return;
                    // int noteNumber = UnityEngine.Random.Range(0, _notes.Count);
                    // AppearNote(noteNumber);
                    // pmm.NoteSpawner(noteNumber);
                    //ノートやっぱり必要ない。1個で十分
                });
        }
    }

    public void AppearNote(int noteNumber)
    {
        GameObject newnote = Instantiate(_notes[noteNumber]);
        newnote.transform.SetParent(GameObject.Find("NoteParent").transform);
        _notes.RemoveAt(noteNumber);
    }
}