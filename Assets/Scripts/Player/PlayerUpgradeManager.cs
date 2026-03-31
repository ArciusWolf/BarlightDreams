using System.Collections.Generic;
using UnityEngine;

public class PlayerUpgradeManager : MonoBehaviour
{
    public static PlayerUpgradeManager Instance;
    [SerializeField] private bool resetOnStart = false;
    public List<UpgradeData> allUpgrades;
    public float cachedMixDuration = 3f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
    #if UNITY_EDITOR
        if (resetOnStart) ResetUpgrades();
    #endif
        LoadUpgrades();
    }



    public UpgradeData GetData(UpgradeType type)
    {
        return allUpgrades.Find(u => u.type == type);
    }

    public bool CanUpgrade(UpgradeType type)
    {
        var data = GetData(type);
        return data != null && data.currentLevel < data.maxLevel;
    }

    public bool TryUpgrade(UpgradeType type)
    {
        var data = GetData(type);
        if (data == null || data.currentLevel >= data.maxLevel) return false;

        int cost = data.costPerLevel[data.currentLevel];
        if (!PlayerManager.Instance.TrySpendMoney(cost)) return false;

        data.currentLevel++;
        SaveUpgrades();
        return true;
    }

    public float GetValue(UpgradeType type)
    {
        var data = GetData(type);
        return data?.CurrentValue ?? 0f;
    }

    public void SaveUpgrades()
    {
        foreach (var upgrade in allUpgrades)
        {
            string key = $"Upgrade_{upgrade.type}_Level";
        }
    }

    public void LoadUpgrades()
    {
        foreach (var upgrade in allUpgrades)
        {
            string key = $"Upgrade_{upgrade.type}_Level";
            if (PlayerPrefs.HasKey(key))
            {
                upgrade.currentLevel = PlayerPrefs.GetInt(key);
            }
        }

        // After loading upgrades, update player stats
        PlayerManager.Instance?.RecalculateMaxLife();
        RecalculateMixingDuration(3f);
    }


    public void ResetUpgrades()
    {
        foreach (var upgrade in allUpgrades)
        {
            upgrade.currentLevel = 0;
            PlayerPrefs.DeleteKey($"Upgrade_{upgrade.type}_Level");
        }

        Debug.Log("[UPGRADE] All upgrades reset.");
    }


    public void RecalculateMixingDuration(float baseDuration)
    {
        float upgrade = GetValue(UpgradeType.FasterMix);
        cachedMixDuration = baseDuration * (1f - upgrade);
        Debug.Log($"[UPGRADE] Cached mix duration updated: {cachedMixDuration}");
    }
}