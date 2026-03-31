using System.Collections;
using UnityEngine;

public class CustomerTimer : MonoBehaviour
{
    [Header("Timer Visual")]
    [SerializeField] private SpriteRenderer timerRenderer;
    [SerializeField] private Sprite[] timerSprites;

    private Coroutine currentRoutine;
    private bool isRunning = false;
    private TimerType currentType;

    public bool IsRunning
    {
        get { return isRunning; }
    }

    public TimerType CurrentType
    {
        get { return currentType; }
    }

    public void Setup(SpriteRenderer renderer, Sprite[] sprites)
    {
        timerRenderer = renderer;
        timerSprites = sprites;
    }

    // ======================
    // === PUBLIC METHODS ===
    // ======================

    public void StartInteractTimer(MonoBehaviour host, float duration, System.Action onComplete)
    {
        StartGenericTimer(host, duration, onComplete, TimerType.Interact);
    }

    public void StartOrderTimer(MonoBehaviour host, float duration, System.Action onComplete)
    {
        StartGenericTimer(host, duration, onComplete, TimerType.Order);
    }

    public void StopTimer(MonoBehaviour host)
    {
        if (currentRoutine != null)
        {
            host.StopCoroutine(currentRoutine);
            currentRoutine = null;
        }

        if (timerRenderer != null)
        {
            timerRenderer.gameObject.SetActive(false);
        }

        isRunning = false;
        currentType = TimerType.None;
    }

    // ==========================
    // === INTERNAL FUNCTIONS ===
    // ==========================

    private void StartGenericTimer(MonoBehaviour host, float duration, System.Action onComplete, TimerType type)
    {
        if (timerRenderer == null || timerSprites == null || timerSprites.Length == 0)
        {
            Debug.LogWarning("Timer setup is incomplete. Missing renderer or sprites.");
            return;
        }

        StopTimer(host);

        currentRoutine = host.StartCoroutine(TimerRoutine(duration, onComplete, type));
        currentType = type;
    }

    private IEnumerator TimerRoutine(float duration, System.Action onComplete, TimerType type)
    {
        if (timerRenderer == null)
        {
            yield break;
        }

        timerRenderer.gameObject.SetActive(true);
        isRunning = true;

        float elapsed = 0f;
        int totalStages = timerSprites.Length;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            int index = Mathf.FloorToInt((elapsed / duration) * totalStages);
            index = Mathf.Clamp(index, 0, totalStages - 1);
            timerRenderer.sprite = timerSprites[index];

            yield return null;
        }

        timerRenderer.gameObject.SetActive(false);
        isRunning = false;
        currentType = TimerType.None;

        if (onComplete != null)
        {
            onComplete.Invoke();
        }
    }
}

// =======================
// === SUPPORTING ENUM ===
// =======================
public enum TimerType
{
    None,
    Interact,
    Order
}
