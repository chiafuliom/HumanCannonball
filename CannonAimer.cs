using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class CannonAimer : MonoBehaviour
{
    [Header("Scene References")]
    [Tooltip("The Transform that rotates around Z to aim (usually the cannon barrel).")]
    public Transform barrel;

    [Tooltip("UI Slider with min=0, max=90 (degrees).")]
    public Slider angleSlider;

    [Tooltip("Text that displays the angle (0–90°). TextMeshProUGUI preferred.")]

    public TextMeshProUGUI angleLabel;



    [Header("Options")]
    [Tooltip("Start angle in degrees (0 = straight ahead, 90 = straight up).")]
    [Range(0f, 90f)] public float startAngle = 45f;

    void Awake()
    {
        if (angleSlider != null)
        {
            // Enforce the 0..90 limits on the slider itself
            angleSlider.minValue = 0f;
            angleSlider.maxValue = 90f;

            // Wire the callback once
            angleSlider.onValueChanged.AddListener(OnSliderChanged);
        }
    }

    void Start()
    {
        // Initialize UI from startAngle (clamped)
        float clamped = Mathf.Clamp(startAngle, 0f, 90f);
        if (angleSlider != null) angleSlider.value = clamped;
        ApplyAngle(clamped);
    }

    void OnDestroy()
    {
        if (angleSlider != null)
            angleSlider.onValueChanged.RemoveListener(OnSliderChanged);
    }

    private void OnSliderChanged(float degrees)
    {
        // degrees is 0..90 from the slider
        ApplyAngle(degrees);
    }

    private void ApplyAngle(float degrees)
    {
        // Convert game angle (0..90) to Unity Z rotation using your mapping:
        // Z = degrees - 90  (90 -> 0, 0 -> -90)
        float z = degrees - 90f;

        if (barrel != null)
            barrel.localRotation = Quaternion.Euler(0f, 0f, z);

        if (angleLabel != null)
            angleLabel.text = "Angle: " + Mathf.RoundToInt(degrees).ToString() + "°";
    }
/*
    // Optional: call this if the barrel is rotated elsewhere and you want the slider to follow.
    public void SyncSliderFromBarrel()
    {
        if (barrel == null || angleSlider == null) return;

        // Normalize Z so negatives are preserved (-90..0 range expected)
        float z = barrel.localEulerAngles.z;
        if (z > 180f) z -= 360f; // convert 0..360 to -180..180

        // Invert mapping: degrees = z + 90
        float degrees = Mathf.Clamp(z + 90f, 0f, 90f);
        angleSlider.SetValueWithoutNotify(degrees);
        if (angleLabel != null)
            angleLabel.text = Mathf.RoundToInt(degrees).ToString() + "°";
    } */
}