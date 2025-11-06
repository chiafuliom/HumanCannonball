using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class CannonShooter : MonoBehaviour
{
    [Header("Scene References")]
    public Transform barrel;
    public Transform muzzle;
    public GameObject projectilePrefab;
    public Button fireButton;

    [Header("Launch Settings")]
    public float minLaunchSpeed = 15f;
    public float maxLaunchSpeed = 35f;
    public bool inheritBarrelRotation = true;

    [Header("UI")]
    public TextMeshProUGUI launchForceLabel;  // previews NEXT shot's speed

    [Header("Round Manager")]
    public BasketMover basketMover;           // REQUIRED

    [Header("Extras")]
    public AudioSource fireSfx;
    public float cooldown = 0.15f;

    float _nextFireTime = 0f;
    float _nextLaunchSpeed;
    bool canFire = true;

    void Awake()
    {
        if (fireButton != null) fireButton.onClick.AddListener(TryFire);

        if (basketMover != null)
        {
            basketMover.OnRoundResolved += HandleRoundResolved;
            basketMover.OnShuffleComplete += HandleShuffleComplete;
        }

        if (maxLaunchSpeed < minLaunchSpeed) maxLaunchSpeed = minLaunchSpeed;
    }

    void Start()
    {
        RollNextSpeed();
    }

    void OnDestroy()
    {
        if (fireButton != null) fireButton.onClick.RemoveListener(TryFire);
        if (basketMover != null)
        {
            basketMover.OnRoundResolved -= HandleRoundResolved;
            basketMover.OnShuffleComplete -= HandleShuffleComplete;
        }
    }

    void Update()
    {
        // Touch (Android / iOS)
        if (Input.touchCount > 0)
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                var t = Input.GetTouch(i);
                if (t.phase == TouchPhase.Began && !TouchOverUI(t)) { TryFire(); break; }
            }
        }

#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBGL
        // Mouse fallback for Editor testing
        bool overUI = EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
        if (!overUI && Input.GetButtonDown("Fire1")) TryFire();
#endif
    }

    bool TouchOverUI(Touch t)
    {
        if (EventSystem.current == null) return false;
        return EventSystem.current.IsPointerOverGameObject(t.fingerId);
    }

    // ✅ Public entry (Button should call THIS)
    public void TryFire()
    {
        if (!canFire) return;                 // one shot per round
        if (Time.time < _nextFireTime) return;
        Fire();                               // private — can’t be called from Button by mistake
    }

    // ⛔ Keep private so OnClick can’t bypass canFire
    void Fire()
    {
        if (barrel == null || muzzle == null || projectilePrefab == null || basketMover == null) return;

        canFire = false;                      // lock round
        _nextFireTime = Time.time + cooldown;

        float launchSpeed = _nextLaunchSpeed; // use the previewed speed

        // Spawn projectile
        Quaternion spawnRot = inheritBarrelRotation ? barrel.rotation : Quaternion.identity;
        GameObject proj = Instantiate(projectilePrefab, muzzle.position, spawnRot);

        var rb = proj.GetComponent<Rigidbody2D>();
        if (rb == null) rb = proj.AddComponent<Rigidbody2D>();

        // Aim
        Vector2 dir = ((Vector2)(muzzle.position - barrel.position)).normalized;

        // Your environment: use linearVelocity
        rb.linearVelocity = dir * launchSpeed;
        rb.angularVelocity = 0f;

        if (fireSfx != null) fireSfx.Play();

        // Now we wait for basket/floor to resolve the round.
    }

    void HandleRoundResolved()
    {
        // After catch OR floor miss → wait, then shuffle
        basketMover.MoveToRandomXWithDelay();
    }

    void HandleShuffleComplete()
    {
        // New round
        basketMover.ResetBasket();
        RollNextSpeed();   // preview next shot
        canFire = true;    // unlock
    }

    void RollNextSpeed()
    {
        _nextLaunchSpeed = UnityEngine.Random.Range(minLaunchSpeed, maxLaunchSpeed);
        if (launchForceLabel != null) launchForceLabel.text = $"{_nextLaunchSpeed:0.0} m/s";
    }
}
