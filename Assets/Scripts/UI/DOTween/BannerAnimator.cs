using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class BannerAnimator : MonoBehaviour
{
    [Header("UI References")]
    public RectTransform backgroundBar;
    public RectTransform ribbon;
    public TextMeshProUGUI labelText;

    [Header("Timings")]
    public float backgroundDuration = 0.5f;
    public float ribbonDuration = 0.5f;
    public float textDuration = 0.3f;
    public float delayBetween = 0.15f;
    public float holdDuration = 2f;

    private Vector2 backgroundStartPos;
    private Vector2 ribbonStartPos;
    private float canvasWidth;

    public void AnimateBanner(string label, System.Action onComplete)
    {
        gameObject.SetActive(true);
        labelText.text = label;

        canvasWidth = ((RectTransform)transform).rect.width;

        backgroundStartPos = backgroundBar.anchoredPosition;
        ribbonStartPos = ribbon.anchoredPosition;

        backgroundBar.anchoredPosition = new Vector2(-canvasWidth, backgroundStartPos.y);
        ribbon.anchoredPosition = new Vector2(-canvasWidth, ribbonStartPos.y);
        labelText.alpha = 0;

        Sequence sequence = DOTween.Sequence();

        sequence.Append(backgroundBar.DOAnchorPosX(backgroundStartPos.x, backgroundDuration).SetEase(Ease.OutCubic)).SetUpdate(true);
        sequence.AppendInterval(delayBetween).SetUpdate(true);
        sequence.Append(ribbon.DOAnchorPosX(ribbonStartPos.x, ribbonDuration).SetEase(Ease.OutCubic)).SetUpdate(true);
        sequence.AppendInterval(delayBetween).SetUpdate(true);
        sequence.Append(labelText.DOFade(1f, textDuration)).SetUpdate(true);
        sequence.AppendInterval(holdDuration).SetUpdate(true);
        sequence.Append(labelText.DOFade(0f, textDuration / 2f)).SetUpdate(true);
        sequence.Join(backgroundBar.DOAnchorPosX(canvasWidth, backgroundDuration).SetEase(Ease.InCubic)).SetUpdate(true);
        sequence.Join(ribbon.DOAnchorPosX(canvasWidth, ribbonDuration).SetEase(Ease.InCubic)).SetUpdate(true);

        sequence.OnComplete(() =>
        {
            gameObject.SetActive(false);
            onComplete?.Invoke();
            Time.timeScale = 1f;
        });
    }

    public void AnimateAndThen(string label, System.Action onComplete)
    {
        AnimateBanner(label, onComplete);
        DOTween.Sequence().AppendInterval(backgroundDuration + ribbonDuration + delayBetween * 2 + textDuration + holdDuration + backgroundDuration)
            .OnComplete(() => onComplete?.Invoke());
    }
}
