using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Linq;

public class PaperNoteManager : MonoBehaviour
{
    [SerializeField, TextArea(1, 2)] private string itemName;
    [SerializeField] private Sprite noteImage;
    [SerializeField] private Sprite noteImage_en;
    [SerializeField] private GameObject droppedNote;
    [SerializeField] private GameObject note, mainImage;
    [SerializeField] private float fadeInDuration = 1f, primaryNoteSize = 0.2f, expandDuration = 1.5f;
    [SerializeField] private AudioClip audioclip;
    [SerializeField] private GameObject bellIcon;
    [System.Serializable]
    public class PosRange
    {
        public float
            minx = -500f,
            maxx = 800,
            miny = -350,
            maxy = 100;
    }
    [SerializeField] private PosRange noteDroppedPosRange;

    void Start()
    {
        SetStretchedRectOffset(GetComponent<RectTransform>(), 0,0,0,0);

        //テキスト部分を挿入
        var itemNameText = droppedNote.transform.GetChild(0);//ボタンの一つ目の子にTextがある想定
        if(itemNameText == null) Debug.LogAssertion("ノートの構造確認して！");
        itemNameText.GetComponent<Text>().text = itemName;
        switch(Resources.Load<DebugParameter>("DebugParameter").LanguageType){
            case Language.English : mainImage.GetComponent<Image>().sprite = noteImage_en;
            break;
            default : mainImage.GetComponent<Image>().sprite = noteImage;
            break;

        }
        //ノートボタンとノートを画面上の範囲内にランダムで配置する。
        var targetPos = new Vector2(Random.Range(noteDroppedPosRange.minx, noteDroppedPosRange.maxx),
            Random.Range(noteDroppedPosRange.miny, noteDroppedPosRange.maxy));
        droppedNote.GetComponent<RectTransform>().anchoredPosition = targetPos;
        droppedNote.SetActive(true);
        mainImage.GetComponent<RectTransform>().anchoredPosition = targetPos;
    }

    public void PickUpNote()
    {
        var graphics = note.GetComponentsInChildren<Graphic>();
        var originColors = graphics.Select(c => c.color).ToArray();
        //透明にしとく
        for (int i = 0; i < originColors.Length; i++)
        {
            var newColor = originColors[i];
            newColor.a *= 0;
            graphics[i].color = newColor;
        }
        //グッとフェードイン
        DOTween.To(() => 0, (val) =>
        {
            for (int i = 0; i < originColors.Length; i++)
            {
                var newColor = originColors[i];
                newColor.a *= val;
                graphics[i].color = newColor;
            }
        }, 1f, fadeInDuration);
        //ノートをグッと拡大
        RectTransform mainImageTransform = mainImage.GetComponent<RectTransform>();
        mainImageTransform.localScale = Vector3.one * primaryNoteSize;
        mainImageTransform.DOScale(Vector3.one, expandDuration).SetEase(Ease.InOutQuad);
        //拡大に合わせて位置をグッと真ん中に持ってくる。
        var priPos = mainImageTransform.anchoredPosition;
        DOTween.To(() => 0, (val) =>
        {
            var target = Vector2.Lerp(priPos, Vector2.zero, val);
            mainImageTransform.anchoredPosition = target;
        }, 1f, expandDuration).OnComplete(() =>
        {
            if (audioclip == null) return;
            note.GetComponent<AudioSource>().PlayOneShot(audioclip);
        });
        //FOVもズームインする
        FindObjectOfType<ChangeFOV>().ZoomIn();
        //アクティブにする
        note.SetActive(true);
    }

    public void DestroyThisObj()
    {
        if (bellIcon != null)//Note4の場合、ベルを使用可能にする。
        {
            bellIcon.SetActive(true);
        }
        FindObjectOfType<ChangeFOV>().ZoomOut();
        Destroy(gameObject);
    }
    public static void SetStretchedRectOffset(RectTransform rectT, float left, float top, float right, float bottom) {
        rectT.offsetMin = new Vector2(left, bottom);
        rectT.offsetMax = new Vector2(-right, -top);
    }
}