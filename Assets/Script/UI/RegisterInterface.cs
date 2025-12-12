using UnityEngine;

public class RegisterInterface : MonoBehaviour
{
    void Start()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.uiInterface = this.gameObject;
        }
    }
}