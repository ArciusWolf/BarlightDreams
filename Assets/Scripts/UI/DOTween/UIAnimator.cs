using UnityEngine;
using DG.Tweening;

public static class UIAnimator
{
    public static void Show(GameObject panel, float duration = 0.4f)
    {
        panel.SetActive(true);

        RectTransform rt = panel.GetComponent<RectTransform>();
        CanvasGroup canvasGroup = panel.GetComponent<CanvasGroup>();

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0;
            canvasGroup.DOFade(1, duration).SetUpdate(true);
        }

        if (rt != null)
        {
            rt.anchoredPosition = new Vector2(0, -1000);
            rt.DOAnchorPos(new Vector2(0, 30), duration * 0.75f)
              .SetEase(Ease.OutCubic)
              .SetUpdate(true)
              .OnComplete(() =>
              {
                  rt.DOAnchorPos(Vector2.zero, duration * 0.25f)
                    .SetEase(Ease.OutQuad)
                    .SetUpdate(true);
              });
        }
    }

    public static void Hide(GameObject panel, float duration = 0.25f)
    {
        RectTransform rt = panel.GetComponent<RectTransform>();
        CanvasGroup canvasGroup = panel.GetComponent<CanvasGroup>();

        Sequence s = DOTween.Sequence().SetUpdate(true);

        if (canvasGroup != null)
        {
            s.Join(canvasGroup.DOFade(0, duration));
        }

        if (rt != null)
        {
            s.Join(rt.DOAnchorPos(new Vector2(0, -1000), duration).SetEase(Ease.InCubic));
        }

        s.OnComplete(() =>
        {
            panel.SetActive(false);
            if (rt != null)
                rt.anchoredPosition = Vector2.zero;
        });
    }
}
