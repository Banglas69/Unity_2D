using System.Collections;
using UnityEngine;

public class SlidingDoor : MonoBehaviour
{
    public float openDistance = 3f;
    public float moveSpeed = 3f;

    private Vector3 closedPosition;
    private Vector3 openPosition;
    private Coroutine moveRoutine;
    private bool isOpen;

    private void Awake()
    {
        closedPosition = transform.position;
        openPosition = closedPosition + Vector3.up * openDistance;
    }

    public void Open()
    {
        if (isOpen) return;
        isOpen = true;
        StartMove(openPosition);
    }

    public void Close()
    {
        if (!isOpen) return;
        isOpen = false;
        StartMove(closedPosition);
    }

    public void Toggle()
    {
        if (isOpen) Close();
        else Open();
    }

    private void StartMove(Vector3 targetPosition)
    {
        if (moveRoutine != null)
            StopCoroutine(moveRoutine);

        moveRoutine = StartCoroutine(MoveTo(targetPosition));
    }

    private IEnumerator MoveTo(Vector3 targetPosition)
    {
        while (Vector2.Distance(transform.position, targetPosition) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPosition,
                moveSpeed * Time.deltaTime
            );

            yield return null;
        }

        transform.position = targetPosition;
        moveRoutine = null;
    }
}