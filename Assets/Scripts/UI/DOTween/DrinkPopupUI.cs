using UnityEngine;
using TMPro;
using DG.Tweening;

public class DrinkPopupUI : MonoBehaviour
{
    public static DrinkPopupUI Instance { get; private set; }

    [SerializeField] private RectTransform platePanel;
    [SerializeField] private TextMeshProUGUI drinkNameText;
    [SerializeField] private CanvasGroup drinkNameGroup;

    [Header("Animation Targets")]
    [SerializeField] private float fromX = -847.69f;
    [SerializeField] private float toX = -680.45f;

    [SerializeField] private float fromWidth = 140.67f;
    [SerializeField] private float toWidth = 475.15f;

    [SerializeField] private float slideDuration = 0.4f;
    [SerializeField] private float delayBeforeText = 0.2f;

    private void Awake()
    {
        Instance = this;
        drinkNameGroup.alpha = 0;
    }

    public void Show(string drinkName)
    {
        drinkNameText.text = drinkName;
        drinkNameGroup.alpha = 0;

        platePanel.anchoredPosition = new Vector2(fromX, platePanel.anchoredPosition.y);
        platePanel.sizeDelta = new Vector2(fromWidth, platePanel.sizeDelta.y);

        Sequence s = DOTween.Sequence().SetUpdate(true);

        s.Join(platePanel.DOAnchorPosX(toX, slideDuration).SetEase(Ease.OutCubic));
        s.Join(platePanel.DOSizeDelta(new Vector2(toWidth, platePanel.sizeDelta.y), slideDuration).SetEase(Ease.OutCubic));

        s.AppendCallback(() =>
        {
            drinkNameGroup.DOFade(1, 0.3f).SetUpdate(true);
        });
    }

    public void Hide()
    {
        platePanel.DOAnchorPosX(fromX, 0.3f).SetEase(Ease.InCubic).SetUpdate(true);
        platePanel.DOSizeDelta(new Vector2(fromWidth, platePanel.sizeDelta.y), 0.3f).SetEase(Ease.InCubic).SetUpdate(true);
        drinkNameGroup.alpha = 0;
    }
}
