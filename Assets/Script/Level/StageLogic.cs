using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class StageLogic : MonoBehaviour
{
    [Header("Settings Level")]
    public int levelID = 1;
    public float interactionTime = 1f;
    public float eatingTime = 3f;
    public bool isBusy = false;

    [Header("Speed Upgrades")]
    public int currentPlayerSpeedLevel = 0;
    public int maxPlayerSpeedLevel = 5;
    public float playerSpeedMultiplier = 1.0f;

    public int currentWorkerSpeedLevel = 0;
    public int maxWorkerSpeedLevel = 5;
    public float workerSpeedMultiplier = 1.0f;

    [Header("Price Upgrades")]
    public int maxPriceLevel = 3;
    public Dictionary<ProductType, int> productPriceLevels = new Dictionary<ProductType, int>();
    public Dictionary<ProductType, float> priceMultipliers = new Dictionary<ProductType, float>();

    [Header("Recipe")]
    public List<Recipe> menu;

    [Header("WorkStation")]
    public WorkStation[] mixers;

    [Header("Upgrade Level Limits")]
    public int maxTables = 6;
    public int maxWorkers = 1;

    void Awake()
    {
        if (!priceMultipliers.ContainsKey(ProductType.Limonade)) { priceMultipliers.Add(ProductType.Limonade, 1f); productPriceLevels.Add(ProductType.Limonade, 0); }
        if (!priceMultipliers.ContainsKey(ProductType.Orange)) { priceMultipliers.Add(ProductType.Orange, 1f); productPriceLevels.Add(ProductType.Orange, 0); }
    }

    IEnumerator Start()
    {
        yield return new WaitUntil(() => SaveManager.instance != null);
        yield return null;

        RestoreLevelObjects();
        RestoreStatsUpgrades();
        SyncRecipesWithSave();
    }


    void RestoreStatsUpgrades()
    {
        if (SaveManager.instance == null) return;

        foreach (UpgradeSave item in SaveManager.instance.data.upgrades)
        {
            if (item.id == "Lvl" + levelID + "_PLAYER_SPEED") SetPlayerSpeedLevel(item.count);
            else if (item.id == "Lvl" + levelID + "_WORKER_SPEED") SetWorkerSpeedLevel(item.count);
            else if (item.id == "Lvl" + levelID + "_PRICE_ORANGE") SetProductPriceLevel(ProductType.Orange, item.count);
            else if (item.id == "Lvl" + levelID + "_PRICE_LIMONADE") SetProductPriceLevel(ProductType.Limonade, item.count);
        }
    }

    void SyncRecipesWithSave()
    {
        foreach (Recipe r in menu)
        {
            if (r.type == ProductType.Limonade) r.isUnlocked = true;
            else r.isUnlocked = false;
        }

        if (SaveManager.instance.data.unlockedItems != null)
        {
            foreach (string id in SaveManager.instance.data.unlockedItems)
            {
                if (id.Contains("Orange")) UnlockProduct(ProductType.Orange);
            }
        }
    }

    void RestoreLevelObjects()
    {
        if (SaveManager.instance == null || SaveManager.instance.data == null) return;

        string tableKey = "Lvl" + levelID + "_TABLE";
        string workerKey = "Lvl" + levelID + "_WORKER";

        bool tableFound = false;
        foreach (UpgradeSave item in SaveManager.instance.data.upgrades)
        {
            if (item.id == tableKey)
            {
                SetActiveTables(item.count);
                tableFound = true;
                break;
            }
        }
        if (!tableFound) SetActiveTables(0);

        bool workerFound = false;
        foreach (UpgradeSave item in SaveManager.instance.data.upgrades)
        {
            if (item.id == workerKey)
            {
                SetActiveWorkers(item.count);
                workerFound = true;
                break;
            }
        }
        if (!workerFound) SetActiveWorkers(0);
    }


    public void SetPlayerSpeedLevel(int level)
    {
        currentPlayerSpeedLevel = level;
        playerSpeedMultiplier = 1.0f + (currentPlayerSpeedLevel * 0.2f);
    }

    public void SetWorkerSpeedLevel(int level)
    {
        currentWorkerSpeedLevel = level;
        workerSpeedMultiplier = 1.0f + (currentWorkerSpeedLevel * 0.25f);
    }

    public void SetProductPriceLevel(ProductType type, int level)
    {
        if (productPriceLevels.ContainsKey(type))
        {
            productPriceLevels[type] = level;
            priceMultipliers[type] = 1.0f + (level * 1f);
        }
    }

    void SetActiveTables(int targetCount)
    {
        CustomerSpawner spawner = FindFirstObjectByType<CustomerSpawner>();
        if (spawner != null)
        {
            int current = 0;
            foreach (Transform child in spawner.transform)
                if (child.gameObject.activeSelf) current++;

            int missing = targetCount - current;
            for (int i = 0; i < missing; i++) spawner.IncreaseCapacity();
        }
    }

    void SetActiveWorkers(int targetCount)
    {
        AIWorker[] activeWorkers = FindObjectsByType<AIWorker>(FindObjectsSortMode.None);
        int current = activeWorkers.Length;
        int missing = targetCount - current;

        if (missing > 0)
        {
            AIWorker[] sleepingBots = FindObjectsByType<AIWorker>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            foreach (AIWorker bot in sleepingBots)
            {
                if (missing <= 0) break;
                if (!bot.gameObject.activeSelf)
                {
                    bot.gameObject.SetActive(true);
                    missing--;
                }
            }
        }
    }


    public int GetStatLevel(string type)
    {
        if (type == "PLAYER_SPEED") return currentPlayerSpeedLevel;
        if (type == "WORKER_SPEED") return currentWorkerSpeedLevel;
        if (type == "PRICE_ORANGE") return productPriceLevels.ContainsKey(ProductType.Orange) ? productPriceLevels[ProductType.Orange] : 0;
        if (type == "PRICE_LIMONADE") return productPriceLevels.ContainsKey(ProductType.Limonade) ? productPriceLevels[ProductType.Limonade] : 0;
        return 0;
    }

    public int GetStatMaxLimit(string type)
    {
        if (type == "PLAYER_SPEED") return maxPlayerSpeedLevel;
        if (type == "WORKER_SPEED") return maxWorkerSpeedLevel;
        if (type.Contains("PRICE_")) return maxPriceLevel;
        return 99;
    }


    public Recipe GetRecipe(ProductType type)
    {
        foreach (Recipe r in menu)
        {
            if (r.type == type) return r;
        }
        return null;
    }

    public bool IsProductUnlocked(ProductType type)
    {
        Recipe r = GetRecipe(type);
        return r != null && r.isUnlocked;
    }

    public ProductType GetRandomUnlockedProduct()
    {
        List<ProductType> available = new List<ProductType>();
        foreach (Recipe r in menu)
        {
            if (r.isUnlocked) available.Add(r.type);
        }

        if (available.Count == 0) return ProductType.Limonade;
        return available[Random.Range(0, available.Count)];
    }

    public void UnlockProduct(ProductType type)
    {
        Recipe r = GetRecipe(type);
        if (r != null)
        {
            r.isUnlocked = true;
            Debug.Log("Nouveau produit débloqué : " + type);
        }
    }

    public void CollectMoney(float basePrice, ProductType productType)
    {
        float multiplier = 1.0f;
        if (priceMultipliers.ContainsKey(productType))
        {
            multiplier = priceMultipliers[productType];
        }

        float finalAmount = basePrice * multiplier;

        if (GameManager.instance != null)
        {
            GameManager.instance.GenerateMoney(finalAmount);
        }
    }

    public void StartProduction(float duration, System.Action onFinished)
    {
        StartCoroutine(ProductionRoutine(duration, onFinished));
    }

    IEnumerator ProductionRoutine(float duration, System.Action onFinished)
    {
        isBusy = true;
        yield return new WaitForSeconds(duration);
        isBusy = false;
        onFinished?.Invoke();
    }
}

[System.Serializable]
public class Recipe
{
    public string name;
    public ProductType type;
    public bool isUnlocked;
    public float price;
    public float preparationTime;
    public Sprite icon;
}