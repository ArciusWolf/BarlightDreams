using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WineCounterUI : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject wineCounterUI;
    [SerializeField] private IngredientSO[] allIngredients;
    [SerializeField] private GameObject ingredientPrefab;
    [SerializeField] private Transform ingredientListPanel;

    public void Interact()
    {
        OpenUI();
    }

    public void OpenUI()
    {
        UIAnimator.Show(wineCounterUI, 0.4f);

        ShowIngredientList();
    }


    public void CloseUI()
    {
        UIAnimator.Hide(wineCounterUI, 0.4f);
        SoundManager.Instance.PlaySound(SoundManager.Instance.recipeBookOpenSound[0]);
    }


    public void ShowIngredientList()
    {
        foreach (Transform child in ingredientListPanel)
        {
            Destroy(child.gameObject);
        }

        foreach (var ingredient in allIngredients)
        {
            GameObject ingredientItem = Instantiate(ingredientPrefab, ingredientListPanel);
            ingredientItem.transform.Find("Image/IngreName").GetComponent<TextMeshProUGUI>().text = ingredient.ingredientName;
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
            var selectedIngredients = PlayerManager.Instance.selectedIngredients;

            if (selectedIngredients.Count >= 4)
            {
                NoticeUI.Instance.ShowNotice("You can only hold 4 ingredients!");
                return;
            }

            //Debug.Log("Selected Ingredient: " + selectedIngredient.ingredientName);
            selectedIngredients.Add(selectedIngredient);

            PlayerUI.Instance.UpdateIngredientIcons(selectedIngredients);

            SoundManager.Instance.PlaySound(SoundManager.Instance.ingredientsPickSound[0]);

            for (int i = 0; i < selectedIngredients.Count; i++)
            {
                //Debug.Log("Selected Ingredient " + (i + 1) + ": " + selectedIngredients[i].ingredientName);
            }
        }
        else
        {
            Debug.LogError("Selected ingredient is null.");
        }
    }

    
}
