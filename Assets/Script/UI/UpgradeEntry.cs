using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UpgradeEntry : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI costText;
    public Button buyButton;

    [Header("Settings")]
    public string upgradeName;
    public float baseCost;

    [Header("Configuration")]
    public string upgradeType;


    private StageLogic logic;
    private string saveKey; 
    private float initialBaseCost;

    void Start()
    {
        initialBaseCost = baseCost;

        logic = FindFirstObjectByType<StageLogic>();

        if (logic != null)
        {
            saveKey = "Lvl" + logic.levelID + "_" + upgradeType;

            RestoreState();
        }

        UpdateUI();

        buyButton.onClick.AddListener(OnBuyClicked);
    }

    void RestoreState()
    {
        if (SaveManager.instance == null) return;

        foreach (UpgradeSave item in SaveManager.instance.data.upgrades)
        {
            if (item.id == saveKey)
            {
                int savedCount = item.count;
                int currentActive = CountActiveItems();

                if (savedCount > currentActive)
                {
                    int missing = savedCount - currentActive;
                    for (int i = 0; i < missing; i++)
                    {
                        ForceActivateOneItem();
                    }
                }

                baseCost = initialBaseCost * Mathf.Pow(1.5f, savedCount);

                return;
            }
        }
    }

    void OnBuyClicked()
    {
        if (GameManager.instance.currentMoney >= baseCost)
        {
            BuyUpgrade();
        }
        else
        {
            Debug.Log("Pas assez d'argent ! Manque : " + (baseCost - GameManager.instance.currentMoney));
        }
    }

    void BuyUpgrade()
    {
        GameManager.instance.GenerateMoney(-baseCost);

        baseCost = Mathf.Round(baseCost * 1.5f);

        ForceActivateOneItem();

        SaveProgress();

        UpdateUI();
    }

    void SaveProgress()
    {
        if (SaveManager.instance == null) return;

        int total = CountActiveItems();
        bool found = false;

        foreach (UpgradeSave item in SaveManager.instance.data.upgrades)
        {
            if (item.id == saveKey)
            {
                item.count = total;
                found = true;
                break;
            }
        }

        if (!found)
        {
            SaveManager.instance.data.upgrades.Add(new UpgradeSave(saveKey, total));
        }

        SaveManager.instance.Save();
    }

    int CountActiveItems()
    {
        if (logic == null) return 0;
        int count = 0;

        if (upgradeType == "TABLE")
        {
            CustomerSpawner spawner = FindFirstObjectByType<CustomerSpawner>();
            if (spawner != null)
            {
                foreach (Transform child in spawner.transform)
                    if (child.gameObject.activeSelf) count++;
            }
        }
        else if (upgradeType == "WORKER")
        {
            AIWorker[] activeWorkers = FindObjectsByType<AIWorker>(FindObjectsSortMode.None);
            count = activeWorkers.Length;
        }
        return count;
    }

    void ForceActivateOneItem()
    {
        if (upgradeType == "TABLE")
        {
            CustomerSpawner spawner = FindFirstObjectByType<CustomerSpawner>();
            if (spawner != null) spawner.IncreaseCapacity();
        }
        else if (upgradeType == "WORKER")
        {
            AIWorker[] allBots = FindObjectsByType<AIWorker>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            foreach (AIWorker bot in allBots)
            {
                if (!bot.gameObject.activeSelf)
                {
                    bot.gameObject.SetActive(true);
                    break;
                }
            }
        }
    }

    void UpdateUI()
    {
        if (logic == null) return;

        int currentCount = CountActiveItems();
        int maxLimit = (upgradeType == "TABLE") ? logic.maxTables : logic.maxWorkers;

        if (currentCount >= maxLimit)
        {
            titleText.text = upgradeName + " (MAX)";
            buyButton.interactable = false;
            costText.text = "---";
        }
        else
        {
            titleText.text = upgradeName + " (Lvl " + currentCount + ")";
            costText.text = "$ " + baseCost.ToString("F0");
        }
    }
}