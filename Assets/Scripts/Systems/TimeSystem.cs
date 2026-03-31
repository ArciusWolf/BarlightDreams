using UnityEngine;
using System;

public class TimeSystem : MonoBehaviour
{
    public static TimeSystem Instance { get; private set; }

    [Header("Time Settings")]
    public float timeScale = 96f;
    public int startHour = 18;
    public int endHour = 2;

    public event Action<int, int> OnTimeChanged;
    public event Action OnBarOpen;
    public event Action OnBarClosed;

    public int CurrentDay { get; private set; } = 1;
    public event Action<int> OnDayChanged;


    private float currentTimeMinutes;
    private float minuteTimer = 0f;
    private bool timeActive = true;
    private bool barOpened = false;
    private bool barClosed = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("[TIME] TimeSystem created and marked DontDestroyOnLoad");
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        currentTimeMinutes = startHour * 60f;
        OnTimeChanged?.Invoke(GetHour(), GetMinute());
    }

    private void Update()
    {
        if (!timeActive) return;

        minuteTimer += Time.deltaTime * timeScale;

        if (minuteTimer >= 60f) // 60 seconds = 1 in-game minute
        {
            currentTimeMinutes += 1f;
            minuteTimer = 0f;

            int hour = GetHour();
            int minute = GetMinute();

            OnTimeChanged?.Invoke(hour, minute);
            //Debug.Log($"{hour:D2}:{minute:D2}");

            if (!barOpened && hour == 18 && minute >= 10)
            {
                barOpened = true;
                OnBarOpen?.Invoke();
            }

            if (!barClosed && currentTimeMinutes >= (24 + endHour) * 60)
            {
                barClosed = true;
                timeActive = false;
                OnBarClosed?.Invoke();
            }
        }
    }

    public int GetHour()
    {
        int rawHour = Mathf.FloorToInt(currentTimeMinutes / 60f);
        return rawHour >= 24 ? rawHour - 24 : rawHour;
    }

    public int GetMinute()
    {
        return Mathf.FloorToInt(currentTimeMinutes % 60f);
    }

    public string GetFormattedTime()
    {
        int hour = GetHour();
        int minute = GetMinute();

        int displayHour = hour % 12;
        if (displayHour == 0) displayHour = 12;

        string suffix = (hour >= 12 && hour < 24) ? "PM" : "AM";

        return $"{displayHour:D2}:{minute:D2} {suffix}";
    }

    public void ResetTime()
    {
        currentTimeMinutes = startHour * 60f;
        minuteTimer = 0f;
        timeActive = true;
        barOpened = false;
        barClosed = false;

        OnTimeChanged?.Invoke(GetHour(), GetMinute());
    }

    public void NextDay()
    {
        CurrentDay++;
        ResetTime();

        PlayerManager.Instance.currentLife = PlayerManager.Instance.MaxLife;
        PlayerUI.Instance?.InitLivesDisplay(PlayerManager.Instance.currentLife);

        OnDayChanged?.Invoke(CurrentDay);
        Debug.Log($"[TIME] CurrentDay is {CurrentDay}");
        DailySummaryManager.Instance.ResetDailyStats();
    }

    public void ForceDayRefresh()
    {
        OnDayChanged?.Invoke(CurrentDay);
    }
    public void OverrideDay(int day)
    {
        CurrentDay = day;
        OnDayChanged?.Invoke(CurrentDay);
        Debug.Log($"[TIME] Day set to {CurrentDay}");
    }

}
