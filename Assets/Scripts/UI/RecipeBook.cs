using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RecipeBook : MonoBehaviour
{
    public static RecipeBook Instance { get; private set; }

    [Header("UI")]
    [SerializeField] private GameObject recipeBookUI;
    [SerializeField] private Transform drinkListPanel;
    [SerializeField] private GameObject drinkIconPrefab;

    [Header("Right Page UI")]
    [SerializeField] private Image drinkRecipeIcon;   // icon đồ uống (phải đổi sang Image thay vì GameObject)
    [SerializeField] private TextMeshProUGUI drinkRecipeName;
    [SerializeField] private Transform ingredientContainer; // chỗ chứa danh sách ingredient
    [SerializeField] private GameObject ingredientInfoPrefab;
    [SerializeField] private TextMeshProUGUI drinkPriceText;

    [Header("Data")]
    [SerializeField] private DrinkRecipeSO[] allRecipes;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void OpenUI()
    {
        UIAnimator.Show(recipeBookUI, 0.4f);
        SoundManager.Instance.PlaySound(SoundManager.Instance.recipeBookOpenSound[0]);
        BuildDrinkList();

        if (allRecipes != null && allRecipes.Length > 0)
        {
            ShowRecipe(allRecipes[0]);
        }
    }

    public void CloseUI()
    {
        UIAnimator.Hide(recipeBookUI, 0.4f);
        SoundManager.Instance.PlaySound(SoundManager.Instance.recipeBookOpenSound[0]);

        if (PlayerUI.Instance != null)
        {
            PlayerUI.Instance.Invoke("CheckUI", 0.41f);
        }
    }

    private void BuildDrinkList()
    {
        foreach (Transform child in drinkListPanel)
        {
            Destroy(child.gameObject);
        }

        foreach (var recipe in allRecipes)
        {
            GameObject plate = Instantiate(drinkIconPrefab, drinkListPanel);

            // Gán sprite
            Transform iconTransform = plate.transform.Find("DrinkIcon");
            if (iconTransform != null)
            {
                Image iconImage = iconTransform.GetComponent<Image>();
                if (iconImage != null)
                {
                    iconImage.sprite = recipe.drinkSprite;
                }
            }

            // Tìm button có sẵn trong plate
            Button btn = plate.GetComponent<Button>();
            if (btn != null)
            {
                DrinkRecipeSO capturedRecipe = recipe; // tránh closure bug
                btn.onClick.AddListener(() => ShowRecipe(capturedRecipe));
            }
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)drinkListPanel);
    }


    private void ShowRecipe(DrinkRecipeSO recipe)
    {
        // Drink sprite
        if (drinkRecipeIcon != null)
            drinkRecipeIcon.sprite = recipe.drinkSprite;

        // Drink name
        if (drinkRecipeName != null)
            drinkRecipeName.text = recipe.drinkName;

        // Drink price
        if (drinkPriceText != null)
            drinkPriceText.text = recipe.drinkPrice.ToString();

        // Xoá ingredient cũ
        foreach (Transform child in ingredientContainer)
        {
            Destroy(child.gameObject);
        }

        // Thêm ingredient mới
        foreach (var ingredient in recipe.ingredients)
        {
            GameObject ingUI = Instantiate(ingredientInfoPrefab, ingredientContainer);

            // Set icon
            Transform iconTransform = ingUI.transform.Find("Background/Background/Ingredient Icon");
            if (iconTransform != null)
            {
                Image iconImage = iconTransform.GetComponent<Image>();
                if (iconImage != null)
                {
                    iconImage.sprite = ingredient.ingredientSprite;
                }
            }

            // Set name
            Transform nameTransform = ingUI.transform.Find("Background/Ingredient Name");
            if (nameTransform != null)
            {
                TextMeshProUGUI nameText = nameTransform.GetComponent<TextMeshProUGUI>();
                if (nameText != null)
                {
                    nameText.text = ingredient.ingredientName;
                }
            }
        }
    }

}
