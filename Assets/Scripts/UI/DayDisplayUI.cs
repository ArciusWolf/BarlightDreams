using TMPro;
using UnityEngine;

public class DayDisplayUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI dayText;

    private void Start()
    {
        TimeSystem.Instance.OnDayChanged += UpdateDayText;
        UpdateDayText(TimeSystem.Instance.CurrentDay);
    }

    private void OnDestroy()
    {
        if (TimeSystem.Instance != null)
            TimeSystem.Instance.OnDayChanged -= UpdateDayText;
    }

    private void UpdateDayText(int day)
    {
        dayText.text = $"Day {day}";
        Debug.Log($"[UI] Updated dayText to: Day {day}");
    }
}
