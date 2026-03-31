using UnityEngine;

[CreateAssetMenu(fileName = "CustomerQuotes", menuName = "Scriptable Objects/Customer Quotes")]
public class CustomerQuotes : ScriptableObject
{
    [Header("Wrong Drink - Angry")]
    public string[] wrongDrinkQuotesAngry = {
        "Are you serious?!",
        "This isn't what I ordered!",
        "I'm not paying for this!",
        "Ugh, terrible service!"
    };

    [Header("Wrong Drink - Lenient")]
    public string[] wrongDrinkQuotesLenient = {
        "Well... it's okay, I guess.",
        "Not what I asked for, but thanks.",
        "I'll let it slide.",
        "Meh, close enough."
    };

    [Header("Correct Drink")]
    public string[] correctDrinkQuotes = {
        "Perfect! Just what I needed!",
        "You're amazing!",
        "Thanks, that was tasty!"
    };
}
