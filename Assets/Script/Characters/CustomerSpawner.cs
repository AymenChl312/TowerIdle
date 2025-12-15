using UnityEngine;
using System.Collections.Generic;

public class CustomerSpawner : MonoBehaviour
{
    [Header("References")]
    public GameObject customerPrefab;
    public Transform spawnPoint;
    public Transform[] customerSlots;

    [Header("Skins Aleatoires")]
    // GLISSE TES OVERRIDES (Skin_Client_01, etc.) ICI DANS L'INSPECTEUR
    public List<AnimatorOverrideController> customerSkins;

    [Header("Settings")]
    public float spawnRate = 3.0f;
    public int maxCapacity = 1;

    // Variables internes
    private float spawnTimer = 0f;
    private int currentCustomersCount = 0;
    private bool[] isSlotTaken;

    void Start()
    {
        isSlotTaken = new bool[customerSlots.Length];
    }

    void Update()
    {
        spawnTimer += Time.deltaTime;

        if (spawnTimer >= spawnRate)
        {
            spawnTimer = 0f;
            TrySpawnCustomer();
        }
    }

    void TrySpawnCustomer()
    {
        if (currentCustomersCount >= maxCapacity) return;

        List<int> freeIndices = new List<int>();

        for (int i = 0; i < customerSlots.Length; i++)
        {
            if (isSlotTaken[i] == false)
            {
                freeIndices.Add(i);
            }
        }

        if (freeIndices.Count == 0) return;

        int randomPickIndex = Random.Range(0, freeIndices.Count);
        int finalSlotIndex = freeIndices[randomPickIndex];

        isSlotTaken[finalSlotIndex] = true;

        GameObject newCustomer = Instantiate(customerPrefab, spawnPoint.position, Quaternion.identity);

        if (customerSkins.Count > 0)
        {
            Animator customerAnim = newCustomer.GetComponent<Animator>();
            if (customerAnim != null)
            {
                int randomSkinIndex = Random.Range(0, customerSkins.Count);
                customerAnim.runtimeAnimatorController = customerSkins[randomSkinIndex];
            }
        }

        Customer script = newCustomer.GetComponent<Customer>();
        script.Initialize(customerSlots[finalSlotIndex], finalSlotIndex);

        currentCustomersCount++;
        newCustomer.transform.SetParent(this.transform);
    }

    public void CustomerLeft(int slotIndexToFree)
    {
        currentCustomersCount--;

        if (slotIndexToFree >= 0 && slotIndexToFree < isSlotTaken.Length)
        {
            isSlotTaken[slotIndexToFree] = false;
        }
    }

    public void IncreaseCapacity()
    {
        if (maxCapacity < customerSlots.Length)
        {
            maxCapacity++;
            Debug.Log("Spawner Upgrade: Capacity is now " + maxCapacity);
            TrySpawnCustomer();
        }
        else
        {
            Debug.Log("Max physical slots reached!");
        }
    }
}