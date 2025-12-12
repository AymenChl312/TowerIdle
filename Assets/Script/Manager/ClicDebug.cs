using UnityEngine;

public class ClicDebug : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            if (hit.collider != null)
            {
                Debug.Log("J'AI TOUCHÉ : " + hit.collider.gameObject.name);
            }
            else
            {
                Debug.Log("JE N'AI RIEN TOUCHÉ (Le laser passe à travers)");
            }
        }
    }
}