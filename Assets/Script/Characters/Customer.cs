using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class Customer : MonoBehaviour
{
    public enum CustomerState { Walking, Waiting, Eating, Leaving }

    [Header("Order")]
    public ProductType desiredProduct;

    [Header("Settings")]
    public float walkSpeed = 3f;
    public float spawnDelay = 1.0f;

    [Header("Interaction")]
    public Transform interactionPoint;

    [Header("UI")]
    public SimpleProgressBar timeBar;

    [Header("State")]
    public bool isBeingServed = false;

    [Header("Command Think")]
    public GameObject thinkCanvas; 
    public Image bgThink;        
    public Image productIcon;     


    public CustomerState currentState = CustomerState.Walking;

    private Transform assignedSlot;
    private int mySlotIndex; 
    private KitchenUI kitchenRef;

    public void Initialize(Transform targetSlot, int slotIndex)
    {
        assignedSlot = targetSlot;
        mySlotIndex = slotIndex;

        kitchenRef = FindFirstObjectByType<KitchenUI>();

        StageLogic logic = FindFirstObjectByType<StageLogic>();

        if (logic != null)
        {
            desiredProduct = logic.GetRandomUnlockedProduct();
        }
        else
        {
            desiredProduct = ProductType.Limonade;
        }

        UpdateBubbleVisual(logic);

        StartCoroutine(SpawnSequence());
    }

    void UpdateBubbleVisual(StageLogic logic)
    {
        if (logic == null) return;

        Recipe recipe = logic.GetRecipe(desiredProduct);

        if (recipe != null)
        {
            productIcon.sprite = recipe.icon;
        }

        bgThink.color = Color.white;
    }

    public void SetServed()
    {
        if (isBeingServed) return;

        isBeingServed = true;

        if (bgThink != null) bgThink.color = Color.gray;
    }


    IEnumerator SpawnSequence()
    {
        yield return new WaitForSeconds(spawnDelay);
        currentState = CustomerState.Walking;
    }

    void Update()
    {
        if (currentState == CustomerState.Walking && assignedSlot != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, assignedSlot.position, walkSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, assignedSlot.position) < 0.01f)
            {
                SitDown();
            }
        }
    }

    void SitDown()
    {
        currentState = CustomerState.Waiting;
        Debug.Log("Client assis ! Prêt à être cliqué.");
    }

    private void OnMouseDown()
    {
        Debug.Log("CLIC REÇU SUR LE CLIENT !");

        if (isBeingServed) return;

        if (currentState == CustomerState.Waiting && kitchenRef != null)
        {
            Debug.Log("Condition OK : J'appelle la cuisine.");
            kitchenRef.OnCustomerClicked(this);
        }
        else
        {
            Debug.Log("Refusé : État = " + currentState + " / Cuisine trouvée ? " + (kitchenRef != null));
        }
    }

    public void StartEating(float duration, Action onFinished)
    {
        currentState = CustomerState.Eating;

        if (thinkCanvas != null) thinkCanvas.SetActive(false);

        if (timeBar != null)
        {
            timeBar.StartTimer(duration);
        }
        StartCoroutine(EatingRoutine(duration, onFinished));
    }

    IEnumerator EatingRoutine(float duration, Action onFinished)
    {
        Debug.Log("Client : Miam miam...");
        yield return new WaitForSeconds(duration);
        onFinished.Invoke();
        Leave();
    }

    void Leave()
    {
        currentState = CustomerState.Leaving;

        CustomerSpawner spawner = FindFirstObjectByType<CustomerSpawner>();
        if (spawner != null)
        {
            spawner.CustomerLeft(mySlotIndex);
        }

        Destroy(gameObject);
    }
}