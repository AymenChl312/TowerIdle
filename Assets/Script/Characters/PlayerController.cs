using UnityEngine;
using System; 

public class PlayerController : MonoBehaviour
{
    [Header("Waypoints")]
    public Transform mixerPoint;    
    public Transform customerPoint; 
    public Transform idlePoint;   

    [Header("Settings")]
    public float moveSpeed = 5f;

    [Header("UI")]
    public SimpleProgressBar timeBar;

    // Variables internes (ne touche pas)
    private Transform currentTarget;
    private Action onArrivedCallback;
    private bool isMoving = false;
    private float waitTimer = 0f;
    private bool isWaiting = false;

    void Update()
    {
        if (isMoving && currentTarget != null)
        {
            MoveCharacter();
        }
        else if (isWaiting)
        {
            waitTimer -= Time.deltaTime;
            if (waitTimer <= 0)
            {
                StopWaiting();
            }
        }
    }

    void MoveCharacter()
    {
        transform.position = Vector3.MoveTowards(transform.position, currentTarget.position, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, currentTarget.position) < 0.01f)
        {
            StopMoving();
        }
    }

    void StopMoving()
    {
        isMoving = false;
        currentTarget = null;

        if (onArrivedCallback != null)
        {
            Action tempAction = onArrivedCallback;
            onArrivedCallback = null;
            tempAction.Invoke();   
        }
    }


    public void GoToMixer(Action onArrived)
    {
        currentTarget = mixerPoint;
        onArrivedCallback = onArrived;
        isMoving = true;
    }

    public void GoToCustomer(Action onArrived)
    {
        currentTarget = customerPoint;
        onArrivedCallback = onArrived;
        isMoving = true;
    }
    public void GoToSpecificPosition(Transform targetPosition, Action onArrived)
    {
        currentTarget = targetPosition;
        onArrivedCallback = onArrived;
        isMoving = true;
    }

    public void GoToIdle()
    {
        currentTarget = idlePoint;
        onArrivedCallback = null;
        isMoving = true;
    }

    public void Wait(float duration, Action onFinished)
    {
        waitTimer = duration;
        onArrivedCallback = onFinished;
        isWaiting = true;

        if (timeBar != null)
        {
            timeBar.StartTimer(duration);
        }
    }

    void StopWaiting()
    {
        isWaiting = false;
        if (onArrivedCallback != null)
        {
            Action temp = onArrivedCallback;
            onArrivedCallback = null;
            temp.Invoke();
        }
    }
}
