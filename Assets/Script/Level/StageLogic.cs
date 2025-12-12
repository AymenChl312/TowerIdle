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