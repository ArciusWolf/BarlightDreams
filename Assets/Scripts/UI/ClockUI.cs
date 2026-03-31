using UnityEngine;
using TMPro;

public class ClockUI : MonoBehaviour
{
    public TMP_Text clockText;

    void Update()
    {
        if (TimeSystem.Instance != null)
            clockText.text = TimeSystem.Instance.GetFormattedTime();
    }
}
