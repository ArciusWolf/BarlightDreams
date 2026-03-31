using UnityEngine;

[CreateAssetMenu(fileName = "MysteryDrinkSO", menuName = "Scriptable Objects/MysteryDrinkSO")]
public class MysteryDrinkSO : ScriptableObject
{
    [Header("General Info")]
    public string drinkName = "Mystery Drink";
    public string drinkDescription = "A total gamble!";
    public Sprite drinkSprite;

    [Header("Reaction-Based Variants")]
    public AudioClip lovedSFX;
    public AudioClip neutralSFX;
    public AudioClip hatedSFX;

    public string[] lovedQuotes;
    public string[] neutralQuotes;
    public string[] hatedQuotes;
}
