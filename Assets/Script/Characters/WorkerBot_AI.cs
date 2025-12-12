using UnityEngine;
using System.Collections;

public class AIWorker : MonoBehaviour
{
    [Header("References")]
    public PlayerController myController;
    public CustomerSpawner spawner;
    public StageLogic logic;

    private bool isBusy = false;

    void Start()
    {
        if (spawner == null) spawner = FindFirstObjectByType<CustomerSpawner>();
        if (logic == null) logic = FindFirstObjectByType<StageLogic>();
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

        Debug.Log("IA : Prise de commande pour " + target.name);

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
                            logic.CollectMoney(recipe.price);
                        });

                        myController.GoToIdle();
                        isBusy = false;
                    });
                });
            });
        });
    }
}