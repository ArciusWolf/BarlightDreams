using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class FruitCounterUI : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject fruitCounterUI;
    [SerializeField] private IngredientSO[] fruitIngredients;
    [SerializeField] private GameObject ingredientPrefab;
    [SerializeField] private Transform ingredientListPanel;


    public void Interact()
    {
        OpenUI();
    }

    public void OpenUI()
    {
        UIAnimator.Show(fruitCounterUI, 0.4f);
        ShowIngredientList();
    }

    public void CloseUI()
    {
        UIAnimator.Hide(fruitCounterUI, 0.4f);
        SoundManager.Instance.PlaySound(SoundManager.Instance.recipeBookOpenSound[0]);
    }

    public void ShowIngredientList()
    {
        foreach (Transform child in ingredientListPanel)
        {
            Destroy(child.gameObject);
        }

        foreach (var ingredient in fruitIngredients)
        {
            GameObject ingredientItem = Instantiate(ingredientPrefab, ingredientListPanel);
            ingredientItem.transform.Find("IngreName").GetComponent<TextMeshProUGUI>().text = ingredient.ingredientName;
            ingredientItem.transform.Find("IngreSprite").GetComponent<Image>().sprite = ingredient.ingredientSprite;

            Button button = ingredientItem.GetComponent<Button>();
            if (button != null)
            {
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() => OnIngredientSelected(ingredient));
            }
        }
    }

    public void OnIngredientSelected(IngredientSO selectedIngredient)
    {
        if (selectedIngredient != null)
        {
            if (PlayerManager.Instance.selectedIngredients.Count >= 4)
            {
                NoticeUI.Instance.ShowNotice("You can only hold 4 ingredients!");
                return;
            }
            else
            {
                Debug.Log("Selected Ingredient: " + selectedIngredient.ingredientName);
                PlayerManager.Instance.selectedIngredients.Add(selectedIngredient);
                PlayerUI.Instance.UpdateIngredientIcons(PlayerManager.Instance.selectedIngredients);
                SoundManager.Instance.PlaySound(SoundManager.Instance.ingredientsPickSound[0]);
            }
        }
        else
        {
            Debug.LogError("Selected ingredient is null.");
        }
    }

   

}
