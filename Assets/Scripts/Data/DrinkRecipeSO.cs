using UnityEngine;

[CreateAssetMenu(fileName = "DrinkRecipeSO", menuName = "Scriptable Objects/DrinkRecipeSO")]
public class DrinkRecipeSO : ScriptableObject
{
    public string drinkName;
    public string drinkDescription;
    public Sprite drinkSprite;
    public IngredientSO[] ingredients;
    public int drinkPrice;
}
