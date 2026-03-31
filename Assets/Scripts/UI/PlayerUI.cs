using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

public class PlayerUI : MonoBehaviour
{
    public static PlayerUI Instance { get; private set; }

    [Header("UI Setup")]
    [SerializeField] private RectTransform iconHolder;
    [SerializeField] private GameObject iconPrefab;
    [SerializeField] private float iconSpacing = 7f;
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private TextMeshProUGUI drinkNameText;
    [SerializeField] private RectTransform livesHolder;
    [SerializeField] private GameObject cupIconPrefab;

    [Header("UI Containers")]
    [SerializeField] private GameObject wineContainerUI;
    [SerializeField] private GameObject fruitContainerUI;
    [SerializeField] private GameObject userContainerUI;
    [SerializeField] private GameObject recipeContainerUI;
    [SerializeField] private GameObject playerIngredientPlate;
    [SerializeField] private GameObject mixingMinigameUI;

    [Header("Glass Holder")]
    public GameObject glassUIElement;
    public bool hasGlass { get; private set; }

    private List<GameObject> currentIcons = new List<GameObject>();
    private List<GameObject> currentLivesIcons = new List<GameObject>();

    [Header("Game Over")]
    public GameOverUI gameOverUI;

    private Image glassImage;
    public Button mixButton;
    private Sprite defaultGlassSprite;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            if (glassUIElement != null)
            {
                glassImage = glassUIElement.GetComponent<Image>();
                if (glassImage != null)
                {
                    defaultGlassSprite = glassImage.sprite;
                }
            }
            else
            {
                Debug.LogWarning("glassUIElement is not assigned — you're likely in Level_Home.");
            }
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Update()
    {
        if (wineContainerUI == null || fruitContainerUI == null || recipeContainerUI == null || userContainerUI == null)
            return;

        CheckUI();
    }

    private void OnEnable()
    {
        if (PlayerManager.Instance != null)
        {
            UpdateMoneyText();
            InitLivesDisplay(PlayerManager.Instance.currentLife);
        }
    }

    public void ShowMixButton(bool state)
    {
        if (mixButton != null)
        {
            mixButton.gameObject.SetActive(state);
        }
    }

    public void SetHasGlass(bool value)
    {
        hasGlass = value;
        glassUIElement.SetActive(value);

        if (!value && glassImage != null)
        {
            glassImage.sprite = defaultGlassSprite;
        }
    }

    public void SetGlassSprite(Sprite drinkSprite)
    {
        if (glassImage != null)
        {
            glassImage.sprite = drinkSprite;
        }
    }
    public void UpdateIngredientIcons(List<IngredientSO> selectedIngredients, int removedIndex = -1)
    {
        int currentCount = currentIcons.Count;
        int targetCount = Mathf.Min(selectedIngredients.Count, 4);

        // Xoá icon bị remove
        if (removedIndex >= 0 && removedIndex < currentCount)
        {
            GameObject removedIcon = currentIcons[removedIndex];
            RectTransform rt = removedIcon.GetComponent<RectTransform>();

            if (rt != null)
            {
                DOTween.Kill(rt);
                rt.DOAnchorPosY(30f, 0.1f).SetEase(Ease.OutCubic).SetUpdate(true)
                    .OnComplete(() =>
                    {
                        DOTween.Kill(rt);
                        rt.DOAnchorPosY(-200f, 0.15f).SetEase(Ease.InBack).SetUpdate(true)
                            .OnComplete(() =>
                            {
                                DOTween.Kill(removedIcon);
                                Destroy(removedIcon);
                            });
                    });
            }

            currentIcons.RemoveAt(removedIndex);
            currentCount--;
        }

        // Cập nhật icon hiện có
        for (int i = 0; i < currentCount && i < selectedIngredients.Count; i++)
        {
            Image img = currentIcons[i].transform.Find("Image")?.GetComponent<Image>();
            if (img != null)
            {
                img.sprite = selectedIngredients[i].ingredientSprite;
            }

            int index = i;
            Button btn = currentIcons[i].GetComponent<Button>();
            if (btn != null)
            {
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() =>
                {
                    if (wineContainerUI.activeSelf || fruitContainerUI.activeSelf)
                    {
                        if (index < PlayerManager.Instance.selectedIngredients.Count)
                        {
                            PlayerManager.Instance.selectedIngredients.RemoveAt(index);
                            UpdateIngredientIcons(PlayerManager.Instance.selectedIngredients, index);
                        }
                    }
                });
            }
        }

        // Tạo icon mới nếu thiếu
        for (int i = currentCount; i < targetCount; i++)
        {
            if (i >= selectedIngredients.Count) break;

            int index = i;
            GameObject icon = Instantiate(iconPrefab, iconHolder);

            Image img = icon.transform.Find("Image")?.GetComponent<Image>();
            if (img != null)
            {
                img.sprite = selectedIngredients[i].ingredientSprite;
            }

            Button btn = icon.GetComponent<Button>();
            if (btn != null)
            {
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() =>
                {
                    if (wineContainerUI.activeSelf || fruitContainerUI.activeSelf)
                    {
                        if (index < PlayerManager.Instance.selectedIngredients.Count)
                        {
                            PlayerManager.Instance.selectedIngredients.RemoveAt(index);
                            UpdateIngredientIcons(PlayerManager.Instance.selectedIngredients, index);
                        }
                    }
                });
            }

            currentIcons.Add(icon);
        }

        if (currentIcons.Count == 0) return;

        // Tính toán layout ngang
        float iconWidth = currentIcons[0].GetComponent<RectTransform>().sizeDelta.x;
        float totalWidth = iconWidth * targetCount + iconSpacing * (targetCount - 1);
        float startX = -totalWidth / 2f + iconWidth / 2f;

        for (int i = 0; i < currentIcons.Count; i++)
        {
            RectTransform rt = currentIcons[i].GetComponent<RectTransform>();
            float targetX = startX + i * (iconWidth + iconSpacing);

            DOTween.Kill(rt); // Dừng tween cũ nếu có

            if (i >= currentCount) // icon mới thêm
            {
                rt.anchoredPosition = new Vector2(targetX, -150f);

                rt.DOAnchorPos(new Vector2(targetX, 20f), 0.18f).SetEase(Ease.OutCubic).SetUpdate(true)
                    .OnComplete(() =>
                    {
                        DOTween.Kill(rt);
                        rt.DOAnchorPos(new Vector2(targetX, 0f), 0.12f).SetEase(Ease.OutQuad).SetUpdate(true);
                    });
            }
            else // icon đã có
            {
                rt.DOAnchorPosX(targetX, 0.15f).SetEase(Ease.InOutCubic).SetUpdate(true);
            }
        }
    }

    public void ClearIcons()
    {
        foreach (var icon in currentIcons)
        {
            DOTween.Kill(icon);
            Destroy(icon);
        }
        currentIcons.Clear();
    }

    public void UpdateMoneyText()
    {
        if (moneyText != null)
        {
            moneyText.text = PlayerManager.Instance != null
                ? PlayerManager.Instance.moneyHeld.ToString()
                : "0";
        }
    }

    public void LoseLifeDisplay()
    {
        if (currentLivesIcons.Count > 0)
        {
            GameObject lastCup = currentLivesIcons[currentLivesIcons.Count - 1];
            Destroy(lastCup);
            currentLivesIcons.RemoveAt(currentLivesIcons.Count - 1);
        }

        if (PlayerManager.Instance != null && PlayerManager.Instance.currentLife == 0 && gameOverUI != null)
        {
            gameOverUI.Show();
        }
    }

    private void CheckUI()
    {
        if (wineContainerUI == null || fruitContainerUI == null || recipeContainerUI == null || userContainerUI == null || playerIngredientPlate == null)
        {
            Debug.LogWarning("Some UI containers are not assigned. Likely in a non-bar scene like Level_Home.");
            return;
        }

        bool wineOrFruitOpen = wineContainerUI.activeSelf || fruitContainerUI.activeSelf;
        bool recipeOpen = recipeContainerUI.activeSelf;
        bool minigameOngoing = mixingMinigameUI.activeSelf;
        bool anyContainerOpen = wineOrFruitOpen || recipeOpen;

        if (wineOrFruitOpen)
        {
            userContainerUI.SetActive(false);
            recipeContainerUI.SetActive(false);
            playerIngredientPlate.SetActive(true);
        }
        else if (recipeOpen)
        {
            userContainerUI.SetActive(false);
            playerIngredientPlate.SetActive(false);
        }
        else if (minigameOngoing)
        {
            userContainerUI.SetActive(false);
            playerIngredientPlate.SetActive(false);
        }
        else
        {
            userContainerUI.SetActive(true);
            playerIngredientPlate.SetActive(true);
        }
    }

    public void SetDrinkName(string name)
    {
        if (drinkNameText != null)
        {
            drinkNameText.text = name;
        }
    }

    public void InitLivesDisplay(int lives)
    {
        foreach (var cup in currentLivesIcons)
        {
            Destroy(cup);
        }
        currentLivesIcons.Clear();

        for (int i = 0; i < lives; i++)
        {
            GameObject cup = Instantiate(cupIconPrefab, livesHolder);
            currentLivesIcons.Add(cup);
        }
    }
}
