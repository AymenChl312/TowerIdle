using UnityEngine;
using System.Collections;

public class KitchenUI : MonoBehaviour
{
    [Header("References")]
    public PlayerController player;
    public StageLogic linkedLogic;

    public bool commandTaken = false;

    void Start()
    {
        if (linkedLogic == null)
        {
            linkedLogic = GetComponent<StageLogic>();
        }
    }

    public void Setup(StageLogic logicToLink)
    {
        linkedLogic = logicToLink;
    }

    public void OnCustomerClicked(Customer targetCustomer)
    {
        if (commandTaken) return;
        if (linkedLogic == null || linkedLogic.isBusy) return;
        if (targetCustomer.currentState != Customer.CustomerState.Waiting) return;
        if (targetCustomer.isBeingServed) return;

        commandTaken = true;
        targetCustomer.SetServed();

        Debug.Log("DEBUT : Commande prise pour " + targetCustomer.name);

        player.GoToSpecificPosition(targetCustomer.interactionPoint, () =>
        {
            player.Wait(linkedLogic.interactionTime, () =>
            {
                StartCoroutine(FindMixerAndContinue(targetCustomer));
            });
        });
    }

    IEnumerator FindMixerAndContinue(Customer targetCustomer)
    {
        Recipe recipe = linkedLogic.GetRecipe(targetCustomer.desiredProduct);

        WorkStation chosenMixer = null;

        while (chosenMixer == null)
        {
            foreach (WorkStation station in linkedLogic.mixers)
            {
                if (station.gameObject.activeSelf && !station.isOccupied && station.stationType == recipe.type)
                {
                    chosenMixer = station;
                    break;
                }
            }

            if (chosenMixer == null)
            {
                player.GoToIdle();
                yield return new WaitForSeconds(0.5f);
            }
        }

        chosenMixer.SetOccupied(true);

        Debug.Log("Mixeur trouvé (" + chosenMixer.name + ") pour faire : " + recipe.type);

        player.GoToSpecificPosition(chosenMixer.interactionPoint, () =>
        {
            if (player.timeBar != null) player.timeBar.StartTimer(recipe.preparationTime);

            linkedLogic.StartProduction(recipe.preparationTime, () =>
            {
                chosenMixer.SetOccupied(false);

                player.GoToSpecificPosition(targetCustomer.interactionPoint, () =>
                {
                    player.Wait(linkedLogic.interactionTime, () =>
                    {
                        commandTaken = false;
                        player.GoToIdle();

                        targetCustomer.StartEating(linkedLogic.eatingTime, () =>
                        {
                            linkedLogic.CollectMoney(recipe.price);
                        });
                    });
                });
            });
        });
    }
}