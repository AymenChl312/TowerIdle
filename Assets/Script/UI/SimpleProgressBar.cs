using UnityEngine;
using UnityEngine.UI;

public class SimpleProgressBar : MonoBehaviour
{
    [Header("Reference")]
    public Image fillingImage;
    public GameObject canvasParent;

    private float totalTime;
    private float passedTime;
    private bool isActive = false;

    void Start()
    {
        if (canvasParent != null) canvasParent.SetActive(false);
    }

    void Update()
    {
        if (isActive)
        {
            passedTime += Time.deltaTime;

            float ratio = passedTime / totalTime;
            fillingImage.fillAmount = ratio;

            if (passedTime >= totalTime)
            {
                isActive = false;
                canvasParent.SetActive(false);
            }
        }
    }

    public void StartTimer(float secondes)
    {
        totalTime = secondes;
        passedTime = 0f;
        fillingImage.fillAmount = 0f;

        isActive = true;
        canvasParent.SetActive(true);
    }
}