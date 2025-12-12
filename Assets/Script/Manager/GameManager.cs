using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    
    [Header("Bank")]
    public TextMeshProUGUI moneyText;

    [Header("All Logics")]
    public StageLogic[] allStages;

    [Header("Navigation")]
    public GameObject towerView;   
    public Transform levelContainer;
    private GameObject currentOpenLevel; 
    public GameObject levelPrefab;
    public GameObject backgroundMenu;
    public GameObject upgradeShopPanel;
    public GameObject uiInterface;

    public static GameManager instance;

    public double currentMoney
    {
        get
        {
            if (SaveManager.instance != null && SaveManager.instance.data != null)
                return SaveManager.instance.data.currentMoney;
            return 0;
        }
        set
        {
            if (SaveManager.instance != null && SaveManager.instance.data != null)
                SaveManager.instance.data.currentMoney = value;
        }
    }

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        UpdateUI();
    }


    public void GenerateMoney(double amount)
    {
        currentMoney += amount;
        UpdateUI();
    }

    void UpdateUI() 
    {
        if (moneyText != null)
        {
            moneyText.text = currentMoney.ToString("F0") + " $";
        }
    }

    

    public void OpenLevel(int id)
    {
        Debug.Log("Ouverture du niveau " + id);

        towerView.SetActive(false);

        if (backgroundMenu != null)
        {
            backgroundMenu.SetActive(false);
        }

        if (currentOpenLevel != null) Destroy(currentOpenLevel);

        currentOpenLevel = Instantiate(levelPrefab, levelContainer);

        KitchenUI ui = currentOpenLevel.GetComponent<KitchenUI>();
        ui.Setup(allStages[id]); 
    }

    public void CloseLevel()
    {
        if (currentOpenLevel != null)
        {
            Destroy(currentOpenLevel);
        }

        towerView.SetActive(true);

        if (backgroundMenu != null)
        {
            backgroundMenu.SetActive(true);
        }
    }

    public void OpenShopFromLevel()
    {
        if (upgradeShopPanel != null)
        {
            upgradeShopPanel.SetActive(true);
        }
        if (uiInterface != null)
            uiInterface.SetActive(false);
    }

    public void CloseShop()
    {
        if (upgradeShopPanel != null)
        {
            upgradeShopPanel.SetActive(false);
        }
        if (uiInterface != null)
            uiInterface.SetActive(true);
    }
}
