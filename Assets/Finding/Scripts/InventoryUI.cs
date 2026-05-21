// Attach to the player’s inventory panel (Canvas in World Space).
// It creates 4 boxes, each with an icon and a TextMeshProUGUI for the count.
// It also listens to the AnalyticalLabStation to toggle between “count” and “moles” display.

using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [System.Serializable]
    public class InventorySlot
    {
        public ItemType type;
        public Image icon;
        public TMP_Text countText;
    }

    [SerializeField] private InventorySlot[] slots;
    [SerializeField] private string countSuffix = "";
    [SerializeField] private string molesSuffix = " moles";

    private bool showMoles = false;
    private bool canSpawnBalloons = false;

    private void Start()
    {
        foreach (var slot in slots)
        {
            if (slot.icon != null)
                slot.icon.gameObject.SetActive(false);
        }
    }

    private void OnEnable()
    {
        InventoryManager.Instance.OnItemCountChanged += UpdateSlot;
        AnalyticalLabStation.OnLabEnter += OnLabEnter;
        AnalyticalLabStation.OnLabExit += OnLabExit;
        AnalyticalLabStation.OnReactionComplete += OnReactionComplete;

        // Initial refresh using the correct two values
        foreach (var slot in slots)
        {
            int itemCount = InventoryManager.Instance.GetItemCount(slot.type);
            int totalMoles = InventoryManager.Instance.GetMoleCount(slot.type);
            UpdateSlot(slot.type, itemCount, totalMoles);
        }
    }

    private void OnDisable()
    {
        if (InventoryManager.Instance != null)
            InventoryManager.Instance.OnItemCountChanged -= UpdateSlot;
        AnalyticalLabStation.OnLabEnter -= OnLabEnter;
        AnalyticalLabStation.OnLabExit -= OnLabExit;
        AnalyticalLabStation.OnReactionComplete -= OnReactionComplete;
    }

    private void OnLabEnter()
    {
        showMoles = true;
        RefreshAllSlots();
    }

    private void OnLabExit()
    {
        showMoles = false;
        RefreshAllSlots();
    }

    private void OnReactionComplete()
    {
        canSpawnBalloons = true;
        // Refresh the balloon slot with both values
        int itemCount = InventoryManager.Instance.GetItemCount(ItemType.Balloon);
        int totalMoles = InventoryManager.Instance.GetMoleCount(ItemType.Balloon);
        UpdateSlot(ItemType.Balloon, itemCount, totalMoles);
    }

    // UpdateSlot now matches the delegate: Action<ItemType, int, int>
    private void UpdateSlot(ItemType type, int itemCount, int totalMoles)
    {
        foreach (var slot in slots)
        {
            if (slot.type == type)
            {
                int displayValue = showMoles ? totalMoles : itemCount;

                if (slot.icon != null)
                    slot.icon.gameObject.SetActive(displayValue > 0);

                slot.countText.text = GetDisplayText(displayValue);

                if (type == ItemType.Balloon && slot.icon.TryGetComponent<Button>(out var btn))
                    btn.interactable = displayValue > 0 && canSpawnBalloons;

                break;
            }
        }
    }

    private void RefreshAllSlots()
    {
        foreach (var slot in slots)
        {
            int itemCount = InventoryManager.Instance.GetItemCount(slot.type);
            int totalMoles = InventoryManager.Instance.GetMoleCount(slot.type);
            int displayValue = showMoles ? totalMoles : itemCount;

            if (slot.icon != null)
                slot.icon.gameObject.SetActive(displayValue > 0);
            slot.countText.text = GetDisplayText(displayValue);

            if (slot.type == ItemType.Balloon && slot.icon.TryGetComponent<Button>(out var btn))
                btn.interactable = displayValue > 0 && canSpawnBalloons;
        }
    }

    private string GetDisplayText(int count)
    {
        return showMoles ? $"{count}{molesSuffix}" : $"{count}{countSuffix}";
    }
}