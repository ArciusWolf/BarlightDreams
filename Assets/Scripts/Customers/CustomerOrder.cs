using System.Collections.Generic;
using UnityEngine;

public class CustomerOrder : MonoBehaviour
{
    [Header("Customer Drink")]
    [SerializeField] private DrinkRecipeSO currentDrink;
    [SerializeField] private List<DrinkRecipeSO> drinkRecipes;
    [SerializeField] private List<DrinkRecipeSO> orderList = new List<DrinkRecipeSO>();
    private int currentOrderIndex = 0;

    [Header("Feedback Quotes")]
    [SerializeField] private CustomerQuotes quotesData;

    [Header("Order UI")]
    public GameObject thoughtBubble;
    public GameObject questionMark;

    [Header("Emotion Feedback")]
    public GameObject lovedEmotion;
    public GameObject neutralEmotion;
    public GameObject neutral2Emotion;
    public GameObject angryEmotion;

    public bool active = false;
    public DrinkRecipeSO orderedDrink { get; private set; }

    private float drinkPrice;
    private CustomerCore customerCore;
    private bool isServing = false;

    public void SetCore(CustomerCore core)
    {
        customerCore = core;
    }

    public void ShowQuestionMark()
    {
        if (questionMark != null)
        {
            questionMark.SetActive(true);
        }
    }

    public void HideQuestionMark()
    {
        if (questionMark != null)
        {
            questionMark.SetActive(false);
        }
    }

    private void Awake()
    {
        if (quotesData == null)
        {
            quotesData = Resources.Load<CustomerQuotes>("Data/Customers/CustomerQuotes");
        }
    }

    public void InitializeOrder()
    {
        if (drinkRecipes == null || drinkRecipes.Count == 0)
        {
            Debug.LogError("No drink recipes assigned.");
            return;
        }

        HideQuestionMark();
        orderList.Clear();

        int day = 1;
        if (TimeSystem.Instance != null)
        {
            day = TimeSystem.Instance.CurrentDay;
        }

        float t = Mathf.Clamp01((day - 1) / 30f);
        float prob1 = Mathf.Lerp(0.95f, 0.5f, t);
        float prob2 = Mathf.Lerp(0.05f, 0.3f, t);
        float prob3 = Mathf.Lerp(0f, 0.2f, t);

        float roll = Random.value;
        int drinkCount = 1;

        if (roll < prob1)
        {
            drinkCount = 1;
        }
        else if (roll < prob1 + prob2)
        {
            drinkCount = 2;
        }
        else
        {
            drinkCount = 3;
        }

        int count = Mathf.Clamp(drinkCount, 1, 3);

        for (int i = 0; i < count; i++)
        {
            int randomIndex = Random.Range(0, drinkRecipes.Count);
            DrinkRecipeSO drink = drinkRecipes[randomIndex];
            orderList.Add(drink);
        }

        currentOrderIndex = 0;
        NextDrinkOrder();
    }

    private void NextDrinkOrder()
    {
        if (currentOrderIndex >= orderList.Count)
        {
            if (customerCore != null)
            {
                customerCore.LeaveAfterDelay(Random.Range(10f, 15f));
            }

            if (thoughtBubble != null)
            {
                thoughtBubble.SetActive(false);
            }

            active = false;
            return;
        }

        orderedDrink = orderList[currentOrderIndex];
        currentOrderIndex++;

        currentDrink = orderedDrink;
        drinkPrice = orderedDrink.drinkPrice;
        active = true;
        isServing = true;

        UpdateDrinkIcon();

        if (thoughtBubble != null)
        {
            thoughtBubble.SetActive(true);
        }

        SoundManager.Instance.PlayRandomCustomerSound();

        // ✅ Gọi Core bắt đầu đếm giờ chờ đồ uống
        if (customerCore != null)
        {
            customerCore.BeginOrderCountdown();
        }
    }

    public void Serve(DrinkRecipeSO drink)
    {
        if (!active || !isServing)
        {
            return;
        }

        isServing = false;

        if (customerCore != null)
        {
            customerCore.HideAllOrderUI();
        }

        bool correct = false;
        if (drink == orderedDrink)
        {
            correct = true;
        }

        if (correct)
        {
            HandleCorrectServe();
        }
        else
        {
            HandleWrongServe();
        }

        if (currentOrderIndex < orderList.Count)
        {
            float enjoyTime = Random.Range(4f, 8f);
            if (thoughtBubble != null)
            {
                thoughtBubble.SetActive(false);
            }
            Invoke("NextDrinkOrder", enjoyTime);
        }
        else
        {
            FinalizeServe(false);
        }
    }

    public void ServeMysteryDrink()
    {
        if (!active)
        {
            return;
        }

        isServing = false;

        if (customerCore != null)
        {
            customerCore.HideAllOrderUI();
        }

        HandleMysteryServe();
        FinalizeServe(true);
    }

    private void HandleMysteryServe()
    {
        MysteryReaction reaction = GetRandomReaction();
        var mystery = PlayerManager.Instance.mysteryDrinkReference;
        string quote = "";
        int reward = 0;
        float multiplier = GetRewardMultiplier();

        switch (reaction)
        {
            case MysteryReaction.Loved:
                reward = Mathf.RoundToInt(orderedDrink.drinkPrice * Random.Range(1.2f, 2f) * multiplier);
                ShowEmotion(lovedEmotion);
                quote = GetRandom(mystery?.lovedQuotes);
                break;

            case MysteryReaction.Neutral:
                reward = Mathf.RoundToInt(orderedDrink.drinkPrice * (1f - Random.Range(0.2f, 0.75f)) * multiplier);

                if (Random.value < 0.5f)
                {
                    ShowEmotion(neutralEmotion);
                }
                else
                {
                    ShowEmotion(neutral2Emotion);
                }

                quote = GetRandom(mystery?.neutralQuotes);
                break;

            case MysteryReaction.Hated:
                PlayerManager.Instance.LoseLife();
                ShowEmotion(angryEmotion);
                quote = GetRandom(mystery?.hatedQuotes);
                break;
        }

        if (reaction != MysteryReaction.Hated)
        {
            PlayerManager.Instance.AddMoney(reward);
            DailySummaryManager.Instance.AddMoney(reward);
        }

        if (mystery != null)
        {
            PlayerUI.Instance.SetGlassSprite(mystery.drinkSprite);
        }

        if (!string.IsNullOrEmpty(quote))
        {
            if (customerCore != null && customerCore.popupResponse != null)
            {
                customerCore.popupResponse.Spawn(transform.position + Vector3.up * 0.2f, quote);
            }
        }

        PlayerManager.Instance.mysteryDrinkReference = null;
    }

    private string GetRandom(string[] array)
    {
        if (array == null)
        {
            return "";
        }

        if (array.Length == 0)
        {
            return "";
        }

        int index = Random.Range(0, array.Length);
        return array[index];
    }

    private void HandleCorrectServe()
    {
        float bonus = PlayerUpgradeManager.Instance.GetValue(UpgradeType.IncreaseIncome);
        float multiplier = GetRewardMultiplier();
        int reward = Mathf.RoundToInt(orderedDrink.drinkPrice * (1f + bonus) * multiplier);

        float tipChance = 0.2f + PlayerUpgradeManager.Instance.GetValue(UpgradeType.TipChances);
        if (Random.value < tipChance)
        {
            int tip = Mathf.RoundToInt(orderedDrink.drinkPrice * 0.15f);
            reward += tip;
        }

        ShowEmotion(lovedEmotion);
        PlayerManager.Instance.AddMoney(reward);
        DailySummaryManager.Instance.AddMoney(reward);
        ShowQuote(quotesData.correctDrinkQuotes);
    }

    private void HandleWrongServe()
    {
        PlayerManager.Instance.currentLife -= 1;
        ShowEmotion(angryEmotion);
        PlayerManager.Instance.LoseLife();
        ShowQuote(quotesData.wrongDrinkQuotesAngry);
    }

    private void ShowQuote(string[] quotes)
    {
        if (quotes == null)
        {
            return;
        }

        if (quotes.Length == 0)
        {
            return;
        }

        int index = Random.Range(0, quotes.Length);
        string text = quotes[index];

        if (customerCore != null)
        {
            if (customerCore.popupResponse != null)
            {
                Vector3 popupPosition = transform.position + Vector3.up * 0.2f;
                customerCore.popupResponse.Spawn(popupPosition, text);
            }
        }
    }

    private void UpdateDrinkIcon()
    {
        if (thoughtBubble == null)
        {
            return;
        }

        Transform drinkChild = thoughtBubble.transform.Find("Drink");
        if (drinkChild != null)
        {
            SpriteRenderer drinkRenderer = drinkChild.GetComponent<SpriteRenderer>();
            if (drinkRenderer != null)
            {
                drinkRenderer.sprite = currentDrink.drinkSprite;
            }
        }
    }

    private void FinalizeServe(bool quickLeave)
    {
        ResetPlayerUI();

        if (thoughtBubble != null)
        {
            thoughtBubble.SetActive(false);
        }

        float leaveDelay = 0f;
        if (quickLeave)
        {
            leaveDelay = Random.Range(5f, 10f);
        }
        else
        {
            leaveDelay = Random.Range(10f, 15f);
        }

        if (customerCore != null)
        {
            customerCore.LeaveAfterDelay(leaveDelay);
        }

        DailySummaryManager.Instance.AddDrinkServed();
    }

    private void ResetPlayerUI()
    {
        PlayerManager.Instance.currentMix = null;
        PlayerManager.Instance.mysteryDrinkReference = null;
        PlayerUI.Instance.UpdateIngredientIcons(PlayerManager.Instance.selectedIngredients);
        PlayerUI.Instance.SetHasGlass(false);
        PlayerUI.Instance.SetDrinkName("");
        //DrinkPopupUI.Instance.Hide();
    }

    public void HandleFailure(string reason)
    {
        Debug.Log(reason);
        PlayerManager.Instance.LoseLife();
        active = false;

        if (thoughtBubble != null)
        {
            thoughtBubble.SetActive(false);
        }
    }

    public float GetDrinkPrice()
    {
        return drinkPrice;
    }

    private MysteryReaction GetRandomReaction()
    {
        float r = Random.value;
        if (r < 0.2f)
        {
            return MysteryReaction.Loved;
        }
        else if (r < 0.7f)
        {
            return MysteryReaction.Neutral;
        }
        else
        {
            return MysteryReaction.Hated;
        }
    }

    private void ShowEmotion(GameObject emotionObject)
    {
        HideAllEmotions();

        if (emotionObject != null)
        {
            emotionObject.SetActive(true);
        }
    }

    public void HideAllEmotions()
    {
        if (lovedEmotion != null)
        {
            lovedEmotion.SetActive(false);
        }
        if (neutralEmotion != null)
        {
            neutralEmotion.SetActive(false);
        }
        if (neutral2Emotion != null)
        {
            neutral2Emotion.SetActive(false);
        }
        if (angryEmotion != null)
        {
            angryEmotion.SetActive(false);
        }
    }

    private float GetRewardMultiplier()
    {
        if (customerCore == null)
        {
            return 1f;
        }

        switch (customerCore.GetCustomerType())
        {
            case CustomerType.VIP:
                return 1.5f;
            case CustomerType.Special:
                return 2f;
            default:
                return 1f;
        }
    }

}
