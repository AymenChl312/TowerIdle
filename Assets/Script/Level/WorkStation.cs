using UnityEngine;

public class WorkStation : MonoBehaviour
{
    [Header("Configuration")]
    public Transform interactionPoint; 

    [Header("State")]
    public bool isOccupied = false;

    public ProductType stationType;
    public void SetOccupied(bool state)
    {
        isOccupied = state;
    }
}