// A complete SliderWithLabel component that you can attach to any UI slider to control its value and display a label.

using UnityEngine;
using UnityEngine.UI;
using TMPro; // or use UnityEngine.UI.Text if not using TextMeshPro

/// <summary>
/// Wraps a Unity Slider and a Text label, providing a clean API.
/// </summary>
public class SliderWithLabel : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private TextMeshProUGUI label;   // Use Text for standard UI

    [Header("Optional Format")]
    [SerializeField] private string format = "F1";    // e.g., "F0" for integer, "F1" for one decimal
    [SerializeField] private string suffix = "";      // e.g., " moles"

    // Public access to the underlying slider’s onValueChanged event
    public Slider.SliderEvent onValueChanged => slider.onValueChanged;

    // Property for value (same as slider.value)
    public float value
    {
        get => slider.value;
        set
        {
            slider.value = value;
            UpdateLabel();
        }
    }

    // Property for maxValue
    public float maxValue
    {
        get => slider.maxValue;
        set
        {
            slider.maxValue = value;
            // If current value exceeds new max, it will be clamped automatically
            UpdateLabel();
        }
    }

    // Property for minValue (optional)
    public float minValue
    {
        get => slider.minValue;
        set
        {
            slider.minValue = value;
            UpdateLabel();
        }
    }

    private void Start()
    {
        // Listen to slider changes to update the label
        slider.onValueChanged.AddListener(OnSliderValueChanged);
        UpdateLabel();
    }

    private void OnDestroy()
    {
        slider.onValueChanged.RemoveListener(OnSliderValueChanged);
    }

    private void OnSliderValueChanged(float newValue)
    {
        UpdateLabel();
    }

    private void UpdateLabel()
    {
        if (label != null)
        {
            label.text = slider.value.ToString(format) + suffix;
        }
    }

    // Optional: Set the whole number step if needed (for int‑like sliders)
    public void SetWholeNumbers(bool wholeNumbers)
    {
        slider.wholeNumbers = wholeNumbers;
    }
}