/// <summary>
/// Attach to a GameObject that has a Slider and a TextMeshProUGUI label.
/// Automatically controls the slider based on inventory count of a specific item.
/// </summary>

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryItemSlider : MonoBehaviour
{
    [SerializeField] private ItemType itemType;          // The item this slider represents
    [SerializeField] private Slider slider;
    [SerializeField] private TextMeshProUGUI label;
    [SerializeField] private string format = "F1";
    [SerializeField] private string suffix = " moles";  // optional

    private InventoryManager inventory;

    private void Start()
    {
        inventory = InventoryManager.Instance;
        if (inventory == null)
        {
            Debug.LogError("InventoryManager not found in scene.");
            return;
        }

        // Listen to inventory changes (now Action<ItemType, int, int>)
        inventory.OnItemCountChanged += OnInventoryChanged;

        // Also listen to the slider’s own value change to update the label
        slider.onValueChanged.AddListener(OnSliderValueChanged);

        // Initialise with total moles
        Refresh(inventory.GetMoleCount(itemType));
    }

    private void OnDestroy()
    {
        if (inventory != null)
            inventory.OnItemCountChanged -= OnInventoryChanged;

        slider.onValueChanged.RemoveListener(OnSliderValueChanged);
    }

    // Updated signature: (ItemType changedType, int itemCount, int totalMoles)
    private void OnInventoryChanged(ItemType changedType, int itemCount, int totalMoles)
    {
        if (changedType == itemType)
            Refresh(totalMoles);   // we care only about total moles for the slider
    }

    private void OnSliderValueChanged(float newValue)
    {
        UpdateLabel();
    }

    /// <summary>
    /// Call whenever the total moles for this item change.
    /// </summary>
    private void Refresh(int currentTotalMoles)
    {
        bool hasItem = currentTotalMoles > 0;

        // Disable slider if nothing left
        slider.interactable = hasItem;

        // If we just ran out, force slider to 0
        if (!hasItem)
            slider.value = 0f;

        // Clamp max to what you actually have (in moles)
        slider.maxValue = currentTotalMoles;

        // If the current slider value exceeds the new max, the slider auto-clamps,
        // but we update the label to reflect the (possibly clamped) value.
        UpdateLabel();
    }

    private void UpdateLabel()
    {
        if (label == null) return;

        int currentTotalMoles = inventory.GetMoleCount(itemType);
        if (currentTotalMoles > 0)
        {
            // The slider's value is the selected moles to use
            label.text = slider.value.ToString(format) + suffix;
        }
        else
        {
            label.text = "?";
        }
    }
}