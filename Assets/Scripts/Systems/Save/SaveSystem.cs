using System.IO;
using UnityEngine;
using System.Collections.Generic;

public static class SaveSystem
{
    private static readonly string savePath = Path.Combine(Application.persistentDataPath, "save.json");

    public static void SaveGame()
    {
        var data = new SaveData
        {
            currentDay = TimeSystem.Instance.CurrentDay,
            moneyHeld = PlayerManager.Instance.moneyHeld,
            currentLife = PlayerManager.Instance.currentLife,
            upgradeLevels = new Dictionary<string, int>()
        };

        foreach (var upgrade in PlayerUpgradeManager.Instance.allUpgrades)
        {
            data.upgradeLevels[upgrade.type.ToString()] = upgrade.currentLevel;
        }

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(savePath, json);

#if UNITY_EDITOR
        Debug.Log("[SAVE] Saved to JSON:\n" + json);
#endif
    }

    public static void LoadGame()
    {
        if (!File.Exists(savePath))
        {
            Debug.LogWarning("[LOAD] No save file found.");
            return;
        }

        string json = File.ReadAllText(savePath);
        var data = JsonUtility.FromJson<SaveData>(json);

        TimeSystem.Instance.OverrideDay(data.currentDay);
        TimeSystem.Instance.ResetTime();

        PlayerManager.Instance.moneyHeld = data.moneyHeld;
        PlayerManager.Instance.currentLife = data.currentLife;
        PlayerUI.Instance?.UpdateMoneyText();
        PlayerUI.Instance?.InitLivesDisplay(data.currentLife);

        foreach (var upgrade in PlayerUpgradeManager.Instance.allUpgrades)
        {
            if (data.upgradeLevels.TryGetValue(upgrade.type.ToString(), out int level))
            {
                upgrade.currentLevel = level;
            }
        }

        PlayerUpgradeManager.Instance.RecalculateMixingDuration(3f);
        PlayerManager.Instance?.RecalculateMaxLife();

#if UNITY_EDITOR
        Debug.Log("[LOAD] Loaded JSON:\n" + json);
#endif
    }

    public static void ResetSave()
    {
        if (File.Exists(savePath))
        {
            File.Delete(savePath);
        }

        if (PlayerUpgradeManager.Instance != null)
        {
            PlayerUpgradeManager.Instance.ResetUpgrades();
        }

#if UNITY_EDITOR
        Debug.Log("[RESET] Save file deleted.");
#endif
    }
}
