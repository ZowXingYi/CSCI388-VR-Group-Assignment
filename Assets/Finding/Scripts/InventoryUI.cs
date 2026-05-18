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
    private bool canSpawnBalloons = false;   // locked until reaction finishes

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

        foreach (var slot in slots)
            UpdateSlot(slot.type, InventoryManager.Instance.GetCount(slot.type));
    }

    private void OnDisable()
    {
        if (InventoryManager.Instance != null)
            InventoryManager.Instance.OnItemCountChanged -= UpdateSlot;
        AnalyticalLabStation.OnLabEnter -= OnLabEnter;
        AnalyticalLabStation.OnLabExit -= OnLabExit;
        AnalyticalLabStation.OnReactionComplete -= OnReactionComplete;
    }

    private void OnLabEnter() => showMoles = true;
    private void OnLabExit() => showMoles = false;

    private void OnReactionComplete()
    {
        canSpawnBalloons = true;
        // refresh the balloon slot to enable the button
        UpdateSlot(ItemType.Balloon, InventoryManager.Instance.GetCount(ItemType.Balloon));
    }

    private void UpdateSlot(ItemType type, int count)
    {
        foreach (var slot in slots)
        {
            if (slot.type == type)
            {
                if (slot.icon != null)
                    slot.icon.gameObject.SetActive(count > 0);

                slot.countText.text = showMoles
                    ? $"{count}{molesSuffix}"
                    : $"{count}{countSuffix}";

                if (type == ItemType.Balloon && slot.icon.TryGetComponent<Button>(out var btn))
                    btn.interactable = count > 0 && canSpawnBalloons;

                break;
            }
        }

        if (showMoles)
        {
            foreach (var slot in slots)
                slot.countText.text = showMoles
                    ? $"{InventoryManager.Instance.GetCount(slot.type)}{molesSuffix}"
                    : $"{InventoryManager.Instance.GetCount(slot.type)}{countSuffix}";
        }
    }
}