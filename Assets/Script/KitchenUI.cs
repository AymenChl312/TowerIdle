using UnityEngine;

public class KitchenUI : MonoBehaviour
{
    private StageLogic linkedLogic;

    public void Setup(StageLogic logicToLink)
    {
        linkedLogic = logicToLink;
        Debug.Log("UI connected to: " + linkedLogic.productName);
    }

    public void OnMixerClicked()
    {
        if (linkedLogic != null)
        {
            linkedLogic.StartCommand();
        }
        else
        {
            Debug.LogError("Error: UI not connected to logic!");
        }
    }

    public void OnReturnClicked()
    {
        GameManager.instance.CloseLevel();
    }
}