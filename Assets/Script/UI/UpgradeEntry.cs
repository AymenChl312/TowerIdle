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
    public int currentLevel = 1;

    public string upgradeType;

    void Start()
    {
        UpdateUI();

        buyButton.onClick.AddListener(OnBuyClicked);
    }

    void UpdateUI()
    {
        titleText.text = upgradeName + " (Lvl " + currentLevel + ")";
        costText.text = "$ " + baseCost.ToString("F0");
    }

    void OnBuyClicked()
    {
        double currentMoney = GameManager.instance.currentMoney;

        if (currentMoney >= baseCost)
        {
            BuyUpgrade();
        }
        else
        {
            Debug.Log("Not enough money! Need: " + baseCost);
            // TODO: Play error sound or flash red
        }
    }

    void BuyUpgrade()
    {
        GameManager.instance.GenerateMoney(-baseCost);

        currentLevel++;

        baseCost = baseCost * 1.5f;

        // 4. Apply Effect (We will code this part in the next step!)
        Debug.Log("Upgrade Purchased: " + upgradeType);
        ApplyUpgradeEffect();

        UpdateUI();
    }

    void ApplyUpgradeEffect()
    {
        if (upgradeType == "TABLE")
        {
            CustomerSpawner spawner = FindFirstObjectByType<CustomerSpawner>();

            if (spawner != null)
            {
                spawner.IncreaseCapacity();
            }
            else
            {
                Debug.LogError("Erreur : Impossible de trouver le CustomerSpawner !");
            }
        }

        else if (upgradeType == "WORKER")
        {
            // Astuce : On doit chercher l'employé même s'il est caché (Inactive) !
            // "FindObjectsInactive.Include" est la clé magique ici.
            AIWorker bot = FindFirstObjectByType<AIWorker>(FindObjectsInactive.Include);

            if (bot != null)
            {
                bot.gameObject.SetActive(true);
                Debug.Log("Upgrade : Employé embauché !");

                buyButton.interactable = false;
                titleText.text = upgradeName + " (MAX)";
            }
            else
            {
                Debug.LogError("Erreur : Impossible de trouver l'Employe_Bot dans la scène !");
            }
        }

        //Autre cas plus tard
    }
}