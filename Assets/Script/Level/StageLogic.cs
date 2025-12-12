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

    [Header("Recipe")]
    public List<Recipe> menu;

    [Header("WorkStation")]
    public WorkStation[] mixers;

    [Header("Upgrade Level Limits")]
    public int maxTables = 6; 
    public int maxWorkers = 1;

    IEnumerator Start()
    {
        yield return new WaitUntil(() => SaveManager.instance != null);
        yield return null;

        RestoreLevelObjects();

        SyncRecipesWithSave();
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

        if (!tableFound)
        {
            SetActiveTables(0);
            Debug.Log("Aucune save table trouvée -> Chargement par défaut (1)");
        }

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

        if (!workerFound)
        {
            SetActiveWorkers(0);
            Debug.Log("Aucune save worker trouvée -> Chargement par défaut (0)");
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
            for (int i = 0; i < missing; i++)
            {
                spawner.IncreaseCapacity();
            }
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

    public Recipe GetRecipe(ProductType type)
    {
        foreach (Recipe r in menu)
        {
            if (r.type == type) return r;
        }
        return null;
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

    public void CollectMoney(float amount)
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.GenerateMoney(amount);
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