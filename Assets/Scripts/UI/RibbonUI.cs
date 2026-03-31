using System;
using System.Collections;
using UnityEngine;

public class RibbonUI : MonoBehaviour
{
    [SerializeField] private BannerAnimator startBanner;
    [SerializeField] private BannerAnimator endBanner;

    public static bool IsBannerAnimating { get; private set; }

    private void Start()
    {
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "Level_Bar")
        {
            PlayStartShift();
        }
    }

    public void PlayStartShift()
    {
        //Time.timeScale = 0f;
        IsBannerAnimating = true;
        startBanner.AnimateBanner("<wave>Shift Started!</wave>", () =>
        {
            IsBannerAnimating = false;
        });
    }

    public void PlayEndingShift(System.Action onComplete)
    {
        Time.timeScale = 0f;
        IsBannerAnimating = true;

        endBanner.AnimateAndThen("<wave>Shift Complete!</wave>", () =>
        {
            IsBannerAnimating = false;
            onComplete?.Invoke();
        });
    }
    public IEnumerator PlayEndingShiftRoutine()
    {
        Time.timeScale = 0f;
        IsBannerAnimating = true;

        bool isDone = false;
        endBanner.AnimateAndThen("<wave>Shift Complete!</wave>", () =>
        {
            IsBannerAnimating = false;
            isDone = true;
        });

        // Chờ đến khi banner chạy xong
        yield return new WaitUntil(() => isDone);
    }

}
