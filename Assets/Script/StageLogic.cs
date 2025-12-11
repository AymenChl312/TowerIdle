using UnityEngine;

public class StageLogic : MonoBehaviour
{
    [Header("Identity")]
    public int levelID;

    [Header("Stats")]
    public string productName;
    public double productPrice;
    public float productTime;

    [Header("Current State")]
    public bool inPreparation = false;
    private float timer = 0f;

    void Update()
    {
        if (inPreparation)
        {
            timer += Time.deltaTime;

            if (timer >= productTime)
            {
                EndPreparation();
            }
        }
    }

    public void StartCommand()
    {
        if (!inPreparation) 
        {
            inPreparation = true;
            timer = 0f;
            Debug.Log("Start of the preparation : " + productName);
        }
    }

    void EndPreparation()
    {
        inPreparation = false;
        
        GameManager.instance.GenerateMoney(productPrice);
        
        Debug.Log("Command Ready !");
    }
}

