using UnityEngine;
using System;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
public class BasketMover : MonoBehaviour
{
    [Header("Movement Settings")]
    public float minX = -4.7f;
    public float maxX = -0.7f;
    public float yPosition = -3f;
    public float moveDelay = .5f;     // wait after round ends before shuffling
    public float moveDuration = 0.5f;  // slide time

    public event Action OnRoundResolved;     // fired on catch OR miss (miss comes from FloorMiss)
    public event Action OnShuffleComplete;   // after delay+shuffle

    Coroutine moveRoutine;
    bool caughtMan = false;

    public void ResetBasket() => caughtMan = false;

    // Called by Cannon after round resolved to animate shuffle after delay
    public void MoveToRandomXWithDelay()
    {
        if (moveRoutine != null) StopCoroutine(moveRoutine);
        moveRoutine = StartCoroutine(MoveAfterDelay());
    }

    IEnumerator MoveAfterDelay()
    {
        yield return new WaitForSeconds(moveDelay);

        if (!caughtMan || caughtMan)
        {
            float newX = UnityEngine.Random.Range(minX, maxX);
            Vector3 start = transform.position;
            Vector3 end = new Vector3(newX, yPosition, transform.position.z);

            float t = 0f;
            while (t < moveDuration)
            {
                t += Time.deltaTime;
                transform.position = Vector3.Lerp(start, end, Mathf.Clamp01(t / moveDuration));
                yield return null;
            }
            transform.position = end;
        }

        moveRoutine = null;
        OnShuffleComplete?.Invoke();
    }
    
    // ✅ SUCCESSFUL CATCH (top/open area set to TRIGGER)
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;

        caughtMan = true;

        Destroy(collision.gameObject);
        if (ScoreManager.Instance != null) ScoreManager.Instance.AddPoint(1);

        OnRoundResolved?.Invoke(); // tell Cannon the round is decided
    }

    // 🔔 Public API for the floor to signal a miss
    public void ResolveRoundAsMiss()
    {
        OnRoundResolved?.Invoke();
    }
}
