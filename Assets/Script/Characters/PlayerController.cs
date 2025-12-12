using UnityEngine;
using System;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [Header("Waypoints")]
    public Transform idlePoint;

    [Header("Settings")]
    public float moveSpeed = 5f;

    [Header("UI")]
    public SimpleProgressBar timeBar;


    public void GoToSpecificPosition(Transform target, Action onArrived)
    {
        StopAllCoroutines();
        StartCoroutine(MoveOrthogonalRoutine(target.position, onArrived));
    }

    public void GoToIdle()
    {
        if (idlePoint != null)
        {
            GoToSpecificPosition(idlePoint, null);
        }
    }

    IEnumerator MoveOrthogonalRoutine(Vector3 targetPos, Action callback)
    {
        targetPos.z = transform.position.z;

        Vector3 cornerPoint = new Vector3(targetPos.x, transform.position.y, transform.position.z);

        while (Vector3.Distance(transform.position, cornerPoint) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, cornerPoint, moveSpeed * Time.deltaTime);
            yield return null;
        }

        while (Vector3.Distance(transform.position, targetPos) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }

        transform.position = targetPos;

        callback?.Invoke();
    }

    public void Wait(float duration, Action onFinished)
    {
        StartCoroutine(WaitRoutine(duration, onFinished));
    }

    IEnumerator WaitRoutine(float duration, Action onFinished)
    {
        if (timeBar != null)
        {
            timeBar.StartTimer(duration);
        }

        yield return new WaitForSeconds(duration);

        onFinished?.Invoke();
    }
}