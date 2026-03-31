using System.Collections.Generic;

[System.Serializable]
public class SaveData
{
    public int currentDay;
    public int moneyHeld;
    public int currentLife;
    public Dictionary<string, int> upgradeLevels = new Dictionary<string, int>();
}
