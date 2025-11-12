using UnityEngine;
using System.Collections;

public class FloorPlayerReset : MonoBehaviour
{
    [Tooltip("Reference to the BasketMover so we can signal round resolution.")]
    public BasketMover basketMover;
    [Header("Miss FX")]
    public ParticleSystem missFXPrefab;   // assign smoke/dust prefab
    public AudioSource missSfx;           // optional “thud” sound
    public float missDestroyDelay = 1.0f;
    public float fadeOutDuration = 0.4f;
    [Header("Miss Visuals")]
 

   
    Coroutine moveRoutine;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;
        CameraShake shaker = Camera.main.GetComponent<CameraShake>();
        if (shaker != null)
            shaker.Shake(0.25f, 0.1f);   // duration, magnitude
        // Signal the round result immediately so the cannon/basket can proceed.
        if (basketMover != null)
            basketMover.ResolveRoundAsMiss();
       //miss effect
        Vector3 pos = collision.contacts[0].point;
        if (missFXPrefab != null)
            Instantiate(missFXPrefab, pos, Quaternion.identity).Play(true);
        if (missSfx != null)
            missSfx.Play();

        // Freeze the man and start delayed despawn.
        StartCoroutine(DelayedDespawn(collision.gameObject));
    }

    private IEnumerator DelayedDespawn(GameObject target)
    {
      

        // Optional: disable further collisions so it doesn't bump things
        var col = target.GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        // Wait before starting the fade (show the "splat" moment)
        if (missDestroyDelay > 0f)
            yield return new WaitForSeconds(missDestroyDelay);

        // Optional fade-out
        if (fadeOutDuration > 0f)
        {
            var sr = target.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                float t = 0f;
                Color c = sr.color;
                float startA = c.a;

                while (t < fadeOutDuration && target != null)
                {
                    t += Time.deltaTime;
                    float a = Mathf.Lerp(startA, 0f, Mathf.Clamp01(t / fadeOutDuration));
                    sr.color = new Color(c.r, c.g, c.b, a);
                    yield return null;
                }
            }
        }

        if (target != null)
            Destroy(target);
    }
}
