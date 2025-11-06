using UnityEngine;

public class FloorPlayerReset : MonoBehaviour
{
    [Tooltip("Reference to the BasketMover in the scene (so we can signal the round result).")]
    public BasketMover basketMover;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;

        // Missed shot: remove the projectile
       // Destroy(collision.gameObject);

        // Tell the round manager (BasketMover) that the round is resolved as a miss
        if (basketMover != null)
            basketMover.ResolveRoundAsMiss();
        else
            Debug.LogWarning("FloorMiss: basketMover reference not set.");
    }
}
