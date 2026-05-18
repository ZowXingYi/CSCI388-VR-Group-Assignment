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

    // Item types – define your enum in a separate file, e.g. ItemType.cs
    // public enum ItemType { Al, NaOH, H2O, Balloon }

    [SerializeField] private int defaultMoles = 0;   // optional starting counts

    private Dictionary<ItemType, int> itemCounts = new Dictionary<ItemType, int>();

    /// <summary>
    /// Fires whenever an item count changes. Passes the item type and new total.
    /// </summary>
    public event Action<ItemType, int> OnItemCountChanged;

    private void Awake()
    {
        // Singleton setup
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Initialize all enum values to zero
        foreach (ItemType type in Enum.GetValues(typeof(ItemType)))
        {
            itemCounts[type] = defaultMoles;
        }
    }

    /// <summary>
    /// Add a specific amount of an item to the inventory.
    /// </summary>
    public void AddItem(ItemType type, int amount)
    {
        if (amount <= 0) return;

        int current = GetCount(type);
        itemCounts[type] = current + amount;
        OnItemCountChanged?.Invoke(type, itemCounts[type]);
    }

    /// <summary>
    /// Overload for adding a single unit (backward compatibility).
    /// </summary>
    public void AddItem(ItemType type)
    {
        AddItem(type, 1);
    }

    /// <summary>
    /// Remove a specific amount of an item. Clamps count to zero.
    /// </summary>
    public void RemoveItem(ItemType type, int amount)
    {
        if (amount <= 0) return;

        int current = GetCount(type);
        itemCounts[type] = Mathf.Max(0, current - amount);
        OnItemCountChanged?.Invoke(type, itemCounts[type]);
    }

    /// <summary>
    /// Get the current total moles for an item type.
    /// </summary>
    public int GetCount(ItemType type)
    {
        if (itemCounts.TryGetValue(type, out int count))
            return count;
        return 0;
    }

    /// <summary>
    /// Check if we have at least the specified amount.
    /// </summary>
    public bool HasEnough(ItemType type, int amount)
    {
        return GetCount(type) >= amount;
    }

    /// <summary>
    /// Reset all counts to zero (for debugging or new game).
    /// </summary>
    public void ResetAll()
    {
        foreach (ItemType type in Enum.GetValues(typeof(ItemType)))
        {
            itemCounts[type] = 0;
            OnItemCountChanged?.Invoke(type, 0);
        }
    }
}