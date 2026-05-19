// Manages the global inventory and notifies listeners of changes.

using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Simple singleton inventory manager that tracks item counts (moles).
/// Each item type can be added or removed in any amount.
/// </summary>
public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    [SerializeField] private int defaultMoles = 0;   // optional starting moles

    // Separate dictionaries for item count (number of objects) and total moles
    private Dictionary<ItemType, int> itemCounts = new Dictionary<ItemType, int>();
    private Dictionary<ItemType, int> moleCounts = new Dictionary<ItemType, int>();

    // Event now passes both counts so listeners can decide what to show
    public event Action<ItemType, int, int> OnItemCountChanged;   // (type, itemCount, totalMoles)

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        foreach (ItemType type in Enum.GetValues(typeof(ItemType)))
        {
            itemCounts[type] = 0;
            moleCounts[type] = defaultMoles;
        }
    }

    /// <summary>
    /// Add one or more objects of a given type, each contributing a certain number of moles.
    /// </summary>
    /// <param name="type">Item type.</param>
    /// <param name="moleAmount">Total moles added by this collection (e.g., 1 can = 2 moles).</param>
    /// <param name="objectCount">How many individual objects this represents (default 1).</param>
    public void AddItem(ItemType type, int moleAmount, int objectCount = 1)
    {
        if (moleAmount <= 0 || objectCount <= 0) return;

        itemCounts[type] += objectCount;
        moleCounts[type] += moleAmount;

        OnItemCountChanged?.Invoke(type, itemCounts[type], moleCounts[type]);
    }

    // Overload for adding a single unit (1 object, 1 mole)
    public void AddItem(ItemType type)
    {
        AddItem(type, 1, 1);
    }

    public void RemoveItem(ItemType type, int moleAmount, int objectCount = 1)
    {
        if (moleAmount <= 0 || objectCount <= 0) return;

        itemCounts[type] = Mathf.Max(0, itemCounts[type] - objectCount);
        moleCounts[type] = Mathf.Max(0, moleCounts[type] - moleAmount);

        OnItemCountChanged?.Invoke(type, itemCounts[type], moleCounts[type]);
    }

    // Get the number of objects (cans, flasks, etc.)
    public int GetItemCount(ItemType type)
    {
        return itemCounts.TryGetValue(type, out int count) ? count : 0;
    }

    // Get the total moles of that item
    public int GetMoleCount(ItemType type)
    {
        return moleCounts.TryGetValue(type, out int moles) ? moles : 0;
    }

    // Original GetCount now returns moles for backward compatibility (used by HasEnough etc.)
    public int GetCount(ItemType type) => GetMoleCount(type);

    public bool HasEnough(ItemType type, int amount)
    {
        return GetMoleCount(type) >= amount;
    }

    public void ResetAll()
    {
        foreach (ItemType type in Enum.GetValues(typeof(ItemType)))
        {
            itemCounts[type] = 0;
            moleCounts[type] = 0;
            OnItemCountChanged?.Invoke(type, 0, 0);
        }
    }
}