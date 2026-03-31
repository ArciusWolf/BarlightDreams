using UnityEngine;
using DG.Tweening;
using TMPro;

public class NoticeUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private RectTransform noticePlate;     // RectTransform của "Notice"
    [SerializeField] private TextMeshProUGUI noticeText;    // TMP Text

    private Vector2 plateHiddenPos = new Vector2(-984f, -263f);
    private Vector2 plateShownPos = new Vector2(-600f, -263f);
    public static NoticeUI Instance;

    private void Awake()
    {
        Instance = this;
    }

    private bool isShowing = false;

    public void ShowNotice(string message)
    {
        if (isShowing) return;

        isShowing = true;
        gameObject.SetActive(true);
        noticeText.text = message;

        // Reset vị trí ban đầu
        noticePlate.anchoredPosition = plateHiddenPos;
        noticePlate.sizeDelta = new Vector2(0f, 100f);
        noticeText.rectTransform.anchoredPosition = new Vector2(-400f, 0f);
        noticeText.alpha = 0f;

        // Kill tween cũ nếu còn
        DOTween.Kill(noticePlate);
        DOTween.Kill(noticeText.rectTransform);
        DOTween.Kill(noticeText);

        Sequence seq = DOTween.Sequence();

        // Plate trượt vào + mở rộng
        seq.Append(noticePlate.DOAnchorPos(plateShownPos, 0.5f).SetEase(Ease.OutCubic).SetUpdate(true));
        seq.Join(noticePlate.DOSizeDelta(new Vector2(800f, 100f), 0.5f).SetEase(Ease.OutCubic).SetUpdate(true));

        // Chờ chút rồi text trượt vào
        seq.AppendInterval(0.05f).SetUpdate(true);
        seq.Append(noticeText.rectTransform.DOAnchorPos(new Vector2(30f, 0f), 0.5f).SetEase(Ease.OutCubic).SetUpdate(true));
        seq.Join(noticeText.DOFade(1f, 0.4f).SetUpdate(true));

        // Chờ trước khi đóng lại
        seq.AppendInterval(2f).SetUpdate(true);

        // Text trượt ra trái + fade + plate đóng lại cùng lúc
        seq.Append(noticeText.rectTransform.DOAnchorPos(new Vector2(-400f, 0f), 0.4f).SetEase(Ease.InCubic).SetUpdate(true));
        seq.Join(noticeText.DOFade(0f, 0.3f).SetUpdate(true));
        seq.Join(noticePlate.DOAnchorPos(plateHiddenPos, 0.5f).SetEase(Ease.InCubic).SetUpdate(true));
        seq.Join(noticePlate.DOSizeDelta(new Vector2(0f, 100f), 0.5f).SetEase(Ease.InCubic).SetUpdate(true));

        // Kết thúc
        seq.AppendCallback(() =>
        {
            isShowing = false;
            gameObject.SetActive(false);
        }).SetUpdate(true);
    }

}

