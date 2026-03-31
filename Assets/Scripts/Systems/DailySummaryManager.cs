using UnityEngine;

public class DailySummaryManager : MonoBehaviour
{
    public static DailySummaryManager Instance { get; private set; }

    public int drinksServed = 0;
    public int cupsMixed = 0;
    public int moneyEarnedToday = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ResetDailyStats()
    {
        drinksServed = 0;
        cupsMixed = 0;
        moneyEarnedToday = 0;
    }

    public void AddDrinkServed() => drinksServed++;
    public void AddCupMixed() => cupsMixed++;
    public void AddMoney(int amount) => moneyEarnedToday += amount;
}
