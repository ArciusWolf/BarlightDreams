using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeCardUI : MonoBehaviour
{
    [SerializeField] private UpgradeType type;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private Button upgradeButton;

    private UpgradeData data;

    private void Start()
    {
        data = PlayerUpgradeManager.Instance.GetData(type);
        upgradeButton.onClick.AddListener(OnUpgradeClicked);
        Refresh();
    }

    public void Refresh()
    {
        nameText.text = data.displayName;
        descriptionText.text = data.description;
        levelText.text = $"Lv {data.currentLevel}/{data.maxLevel}";

        if (data.currentLevel >= data.maxLevel)
        {
            upgradeButton.interactable = false;
            priceText.text = "MAXED";
        }
        else
        {
            upgradeButton.interactable = true;
            int cost = data.costPerLevel[data.currentLevel];
            priceText.text = cost.ToString();
        }
    }


    private void OnUpgradeClicked()
    {
        bool upgraded = PlayerUpgradeManager.Instance.TryUpgrade(type);
        if (upgraded)
        {
            Refresh();

            if (type == UpgradeType.MaxLife)
            {
                PlayerManager.Instance.currentLife = PlayerManager.Instance.MaxLife;
                PlayerUI.Instance?.InitLivesDisplay(PlayerManager.Instance.currentLife);
            }

            if (type == UpgradeType.FasterMix)
            {
                PlayerUpgradeManager.Instance.RecalculateMixingDuration(3f);
            }
        }

    }
}