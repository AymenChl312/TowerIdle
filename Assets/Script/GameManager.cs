using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    
    [Header("Bank")]
    public double currentMoney = 0;
    public TextMeshProUGUI moneyText;

    [Header("All Logics")]
    public StageLogic[] allStages;

    [Header("Navigation")]
    public GameObject towerView;   
    public Transform levelContainer;
    private GameObject currentOpenLevel; 
    public GameObject levelPrefab;

    public static GameManager instance;

    void Awake()
    {
        instance = this; 
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
        towerView.SetActive(false);
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
    }
}