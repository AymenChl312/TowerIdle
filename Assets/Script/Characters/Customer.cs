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

    private Animator animator;

    public void Initialize(Transform targetSlot, int slotIndex)
    {
        assignedSlot = targetSlot;
        mySlotIndex = slotIndex;
        kitchenRef = FindFirstObjectByType<KitchenUI>();

        animator = GetComponent<Animator>();

        StartCoroutine(InitRoutine());
    }

    IEnumerator InitRoutine()
    {
        yield return null;

        StageLogic logic = FindFirstObjectByType<StageLogic>();

        if (logic != null)
        {
            desiredProduct = logic.GetRandomUnlockedProduct();
            UpdateBubbleVisual(logic);
        }
        else
        {
            desiredProduct = ProductType.Limonade;
        }

        StartCoroutine(SpawnSequence());
    }

    void UpdateBubbleVisual(StageLogic logic)
    {
        if (logic == null) return;
        Recipe recipe = logic.GetRecipe(desiredProduct);
        if (recipe != null) productIcon.sprite = recipe.icon;
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
        if (assignedSlot != null)
        {
            StartCoroutine(MoveOrthogonalRoutine(assignedSlot.position));
        }
    }

    IEnumerator MoveOrthogonalRoutine(Vector3 targetPos)
    {
        if (animator != null) animator.SetBool("isMoving", true);

        targetPos.z = transform.position.z;

        Vector3 cornerPoint = new Vector3(targetPos.x, transform.position.y, transform.position.z);

        float dirX = (targetPos.x > transform.position.x) ? 1f : -1f;
        UpdateAnimDirection(dirX, 0f);

        while (Vector3.Distance(transform.position, cornerPoint) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, cornerPoint, walkSpeed * Time.deltaTime);
            yield return null;
        }

        float dirY = (targetPos.y > transform.position.y) ? 1f : -1f;
        UpdateAnimDirection(0f, dirY);

        while (Vector3.Distance(transform.position, targetPos) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, walkSpeed * Time.deltaTime);
            yield return null;
        }

        transform.position = targetPos;

        SitDown();
    }

    void UpdateAnimDirection(float x, float y)
    {
        if (animator != null)
        {
            animator.SetFloat("InputX", x);
            animator.SetFloat("InputY", y);
        }
    }

    void SitDown()
    {
        if (animator != null) animator.SetBool("isMoving", false);

        currentState = CustomerState.Waiting;
    }


    private void OnMouseDown()
    {
        if (isBeingServed) return;
        if (currentState == CustomerState.Waiting && kitchenRef != null)
        {
            kitchenRef.OnCustomerClicked(this);
        }
    }

    public void StartEating(float duration, Action onFinished)
    {
        currentState = CustomerState.Eating;
        if (thinkCanvas != null) thinkCanvas.SetActive(false);
        if (timeBar != null) timeBar.StartTimer(duration);
        StartCoroutine(EatingRoutine(duration, onFinished));
    }

    IEnumerator EatingRoutine(float duration, Action onFinished)
    {
        yield return new WaitForSeconds(duration);
        onFinished.Invoke();
        Leave();
    }

    void Leave()
    {
        currentState = CustomerState.Leaving;
        CustomerSpawner spawner = FindFirstObjectByType<CustomerSpawner>();
        if (spawner != null) spawner.CustomerLeft(mySlotIndex);
        Destroy(gameObject);
    }
}