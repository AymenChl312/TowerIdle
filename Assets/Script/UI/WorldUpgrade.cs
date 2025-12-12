using UnityEngine;
using TMPro;
using System.Collections;

public class WorldUpgrade : MonoBehaviour
{
    [Header("Settings Price")]
    public float price = 500f;
    public string productName = "Machine";

    public string uniqueID;

    [Header("Recipe Unlock")]
    public bool unlockRecipe = false;
    public ProductType recipeType;

    [Header("References")]
    public GameObject objectInSold;
    public GameObject buyPanel;
    public TextMeshProUGUI priceText;

    void Start()
    {
        if (priceText != null) priceText.text = productName + "\n" + price.ToString() + " $";

        if (SaveManager.instance != null && SaveManager.instance.data != null)
        {
            CheckIfAlreadyBought();
        }
    }

    void CheckIfAlreadyBought()
    {
        if (SaveManager.instance != null)
        {
            if (SaveManager.instance.data.unlockedItems.Contains(uniqueID))
            {
                UnlockContent();

                if (buyPanel != null) buyPanel.SetActive(false);
            }
        }
    }

    public void Buy() 
    {
        if (GameManager.instance.currentMoney >= price)
        {
            GameManager.instance.GenerateMoney(-price);

            UnlockContent();

            if (SaveManager.instance != null)
            {
                if (!SaveManager.instance.data.unlockedItems.Contains(uniqueID))
                {
                    SaveManager.instance.data.unlockedItems.Add(uniqueID);
                    SaveManager.instance.Save(); 
                }
            }

            if (buyPanel != null) buyPanel.SetActive(false);
        }
        else
        {
            Debug.Log("Pas assez d'argent !");
        }
    }

    void UnlockContent()
    {
        if (objectInSold != null) objectInSold.SetActive(true);

        if (unlockRecipe)
        {
            StageLogic logic = FindFirstObjectByType<StageLogic>();
            if (logic != null) logic.UnlockProduct(recipeType);
        }
    }
}