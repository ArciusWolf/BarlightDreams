using System;

[Serializable]
public class UpgradeData
{
    public UpgradeType type;
    public string displayName;
    public string description;
    public int currentLevel = 0;
    public int maxLevel = 10;
    public int[] costPerLevel;
    public float[] valuePerLevel;

    public int CurrentCost => currentLevel < maxLevel ? costPerLevel[currentLevel] : -1;
    public float CurrentValue => currentLevel > 0 ? valuePerLevel[currentLevel - 1] : 0f;
}

