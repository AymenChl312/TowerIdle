using UnityEngine;
using TMPro;

public class WorldUpgrade : MonoBehaviour
{
    [Header("Settings Price")]
    public float price = 500f;
    public string productName = "Machine";

    [Header("Recipe Unlock")]
    public bool unlockRecipe = false; 
    public ProductType recipeType;

    [Header("References")]
    public GameObject objectInSold;
    public GameObject buyPanel;
    public TextMeshProUGUI priceText;

    void Start()
    {
        if (priceText != null)
        {
            priceText.text = productName + "\n" + price.ToString() + " $";
        }
    }

    public void Buy()
    {
        if (GameManager.instance.currentMoney >= price)
        {
            GameManager.instance.GenerateMoney(-price);
            
            if (objectInSold != null)
            {
                objectInSold.SetActive(true);
            }

            if (unlockRecipe)
            {
                StageLogic logic = FindFirstObjectByType<StageLogic>();
                if (logic != null)
                {
                    logic.UnlockProduct(recipeType);
                }
            }

            if (buyPanel != null)
            {
                buyPanel.SetActive(false);
            }
        }
        else
        {
            Debug.Log("Pas assez d'argent !");
        }
    }
}