using UnityEngine;
using TMPro;

public class WorldUpgrade : MonoBehaviour
{
    public enum UpgradeType { ObjectUnlock, PlayerSpeed, WorkerSpeed, ProductPrice }

    [Header("Type d'Amélioration")]
    public UpgradeType type = UpgradeType.ObjectUnlock;

    [Header("Settings Price")]
    public float price = 500f;
    public string productName = "Amélioration";
    public string uniqueID;

    [Header("Pour ObjectUnlock")]
    public GameObject objectInSold;
    public bool unlockRecipe = false;
    public ProductType recipeToUnlock;

    [Header("Pour ProductPrice")]
    public ProductType targetProduct; 
    public ProductType requiredProductUnlocked; 
    public bool requiresCondition = false; 

    [Header("Valeurs")]
    public float multiplierValue = 2.0f; 

    [Header("References")]
    public GameObject buyPanel;
    public TextMeshProUGUI priceText;

    void Start()
    {
        if (priceText != null) priceText.text = productName + "\n" + price.ToString() + " $";

        if (requiresCondition)
        {
            StageLogic logic = FindFirstObjectByType<StageLogic>();
            if (logic != null && !logic.IsProductUnlocked(requiredProductUnlocked))
            {
                gameObject.SetActive(false);
                return; 
            }
        }

        if (SaveManager.instance != null && SaveManager.instance.data != null)
        {
            CheckIfAlreadyBought();
        }
    }

    void CheckIfAlreadyBought()
    {
        if (SaveManager.instance.data.unlockedItems.Contains(uniqueID))
        {
            ApplyEffect();
            if (buyPanel != null) buyPanel.SetActive(false);

            Collider col = GetComponent<Collider>();
            if (col != null) col.enabled = false;
        }
    }

    public void Buy()
    {
        if (GameManager.instance.currentMoney >= price)
        {
            GameManager.instance.GenerateMoney(-price);

            ApplyEffect(); 

            if (SaveManager.instance != null)
            {
                if (!SaveManager.instance.data.unlockedItems.Contains(uniqueID))
                {
                    SaveManager.instance.data.unlockedItems.Add(uniqueID);
                    SaveManager.instance.Save();
                }
            }

            if (buyPanel != null) buyPanel.SetActive(false);
            Collider col = GetComponent<Collider>();
            if (col != null) col.enabled = false;
        }
        else
        {
            Debug.Log("Pas assez d'argent !");
        }
    }

    void ApplyEffect()
    {
        StageLogic logic = FindFirstObjectByType<StageLogic>();
        if (logic == null) return;

        switch (type)
        {
            case UpgradeType.ObjectUnlock:
                if (objectInSold != null) objectInSold.SetActive(true);
                if (unlockRecipe) logic.UnlockProduct(recipeToUnlock);
                break;

            case UpgradeType.PlayerSpeed:
                logic.playerSpeedMultiplier = multiplierValue;
                Debug.Log("Vitesse joueur augmentée !");
                break;

            case UpgradeType.WorkerSpeed:
                logic.workerSpeedMultiplier = multiplierValue;
                Debug.Log("Vitesse workers augmentée !");
                break;

            case UpgradeType.ProductPrice:
                if (logic.priceMultipliers.ContainsKey(targetProduct))
                {
                    logic.priceMultipliers[targetProduct] = multiplierValue;
                }
                else
                {
                    logic.priceMultipliers.Add(targetProduct, multiplierValue);
                }
                Debug.Log("Prix doublé pour : " + targetProduct);
                break;
        }
    }
}