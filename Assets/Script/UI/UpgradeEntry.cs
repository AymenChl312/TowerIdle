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

    // On n'utilise plus 'currentLevel' simple, on le calcule
    public string upgradeType; // "TABLE" ou "WORKER"

    private StageLogic logic;
    private string saveKey; // La clé unique (ex: "Lvl1_TABLE")

    void Start()
    {
        logic = FindFirstObjectByType<StageLogic>();

        if (logic != null)
        {
            // 1. On crée un identifiant unique pour cette upgrade dans ce niveau
            saveKey = "Lvl" + logic.levelID + "_" + upgradeType;

            // 2. ON CHARGE LA SAUVEGARDE
            RestoreState();
        }

        // 3. On met à jour l'affichage
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
                int missing = savedCount - currentActive;

                for (int i = 0; i < missing; i++)
                {
                    ForceActivateOneItem();
                    baseCost = baseCost * 1.5f;
                }
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
            Debug.Log("Pas assez d'argent !");
        }
    }

    void BuyUpgrade()
    {
        GameManager.instance.GenerateMoney(-baseCost);

        baseCost = baseCost * 1.5f;

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