using System.Collections.Generic;

[System.Serializable]
public class SaveData
{
    public double currentMoney;
    public List<UpgradeSave> upgrades;
    public List<string> unlockedItems;

    public List<int> assignedWorkerSkins;

    public SaveData()
    {
        currentMoney = 0;
        upgrades = new List<UpgradeSave>();
        unlockedItems = new List<string>();
        assignedWorkerSkins = new List<int>();
    }
}

[System.Serializable]
public class UpgradeSave
{
    public string id;
    public int count;

    public UpgradeSave(string _id, int _count)
    {
        id = _id;
        count = _count;
    }
}