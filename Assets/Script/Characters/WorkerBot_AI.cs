using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AIWorker : MonoBehaviour
{
    [Header("References")]
    public PlayerController myController;
    public CustomerSpawner spawner;
    public StageLogic logic;

    [Header("WorkerId")]
    public int workerID = 0;

    [Header("Apparence")]
    public List<AnimatorOverrideController> workerSkins;

    private bool isBusy = false;

    void Start()
    {
        if (spawner == null) spawner = FindFirstObjectByType<CustomerSpawner>();
        if (logic == null) logic = FindFirstObjectByType<StageLogic>();

        ApplyPersistentSkin();
    }

    void ApplyPersistentSkin()
    {
        if (workerSkins == null || workerSkins.Count == 0) return;

        Animator anim = myController.GetComponent<Animator>();
        if (anim == null) return;

        int skinIndex = SaveManager.instance.GetOrAssignSkinForWorker(workerID, workerSkins.Count);

        anim.runtimeAnimatorController = workerSkins[skinIndex];
    }

    void Update()
    {
        if (logic == null || spawner == null) return;
        if (isBusy) return;

        FindAndServeCustomer();
    }


    void FindAndServeCustomer()
    {
        Customer[] allCustomers = spawner.GetComponentsInChildren<Customer>();

        foreach (Customer client in allCustomers)
        {
            if (client.currentState == Customer.CustomerState.Waiting && !client.isBeingServed)
            {
                StartJob(client);
                return;
            }
        }
    }

    void StartJob(Customer target)
    {
        isBusy = true;
        target.SetServed();

        myController.GoToSpecificPosition(target.interactionPoint, () =>
        {
            myController.Wait(logic.interactionTime, () =>
            {
                StartCoroutine(FindMixerAndContinue(target));
            });
        });
    }

    IEnumerator FindMixerAndContinue(Customer target)
    {
        Recipe recipe = logic.GetRecipe(target.desiredProduct);

        WorkStation chosenMixer = null;

        while (chosenMixer == null)
        {
            foreach (WorkStation station in logic.mixers)
            {
                if (station.gameObject.activeSelf && !station.isOccupied && station.stationType == recipe.type)
                {
                    chosenMixer = station;
                    break;
                }
            }

            if (chosenMixer == null)
            {
                myController.GoToIdle();
                yield return new WaitForSeconds(0.5f);
            }
        }

        chosenMixer.SetOccupied(true);

        myController.GoToSpecificPosition(chosenMixer.interactionPoint, () =>
        {
            myController.Wait(recipe.preparationTime, () =>
            {
                chosenMixer.SetOccupied(false);

                myController.GoToSpecificPosition(target.interactionPoint, () =>
                {
                    myController.Wait(logic.interactionTime, () =>
                    {
                        target.StartEating(logic.eatingTime, () =>
                        {
                            logic.CollectMoney(recipe.price, recipe.type);
                        });

                        myController.GoToIdle();
                        isBusy = false;
                    });
                });
            });
        });
    }
}