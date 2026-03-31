using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DailySummaryUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI cupsText;
    [SerializeField] private TextMeshProUGUI customersText;
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private GameObject panel;
    [SerializeField] private Button nextButton;

    private Action onCompleteCallback;

    private void Awake()
    {
        nextButton.onClick.RemoveListener(OnNextPressed);
        nextButton.onClick.AddListener(OnNextPressed);
    }

    public void Show(Action callback)
    {
        cupsText.text = $"Cups Mixed: {DailySummaryManager.Instance.cupsMixed}";
        customersText.text = $"Customers Served: {DailySummaryManager.Instance.drinksServed}";
        moneyText.text = $"Money Earned: ${DailySummaryManager.Instance.moneyEarnedToday}";

        onCompleteCallback = callback;

        UIAnimator.Show(panel, 0.4f);
    }

    public void OnNextPressed()
    {
        UIAnimator.Hide(panel, 0.4f);
        onCompleteCallback?.Invoke();
    }
}
