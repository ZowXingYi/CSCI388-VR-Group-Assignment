// Attached to the Balloon inventory slot’s button.
// When pressed, spawns a balloon prefab at the player’s hand and consumes one from inventory.

using UnityEngine;

public class BalloonSpawner : MonoBehaviour
{
    [Header("Balloon Variants (different colours)")]
    [SerializeField] private GameObject[] balloonPrefabs;  // e.g., red, blue, green

    [SerializeField] private Transform handAttachPoint;

    private GameObject currentBalloon;   // keep track so we can destroy it later

    /// <summary>
    /// Spawn a balloon in the hand without changing inventory.
    /// Used after the reaction gives balloons.
    /// </summary>
    public void SpawnBalloonInHand()
    {
        Debug.Log("SpawnBalloonInHand called. Balloon count: " +
              InventoryManager.Instance.GetItemCount(ItemType.Balloon));

        if (InventoryManager.Instance.GetItemCount(ItemType.Balloon) <= 0)
        {
            Debug.LogWarning("No balloons in inventory!");
            return;
        }

        if (currentBalloon != null)
            Destroy(currentBalloon);

        GameObject prefab = balloonPrefabs[Random.Range(0, balloonPrefabs.Length)];
        Debug.Log("Instantiating balloon: " + prefab.name);

        currentBalloon = Instantiate(prefab, handAttachPoint.position, handAttachPoint.rotation, handAttachPoint);

        Debug.Log("Balloon spawned under: " + currentBalloon.transform.parent.name);
    }

    /// <summary>
    /// Call this when the balloon is placed on a door.
    /// Deducts one balloon and immediately spawns the next one (if any left).
    /// </summary>
    public void ConsumeBalloonAndRespawn()
    {
        // Deduct one balloon item
        InventoryManager.Instance.RemoveItem(ItemType.Balloon, 1);

        // Destroy the current balloon
        if (currentBalloon != null)
        {
            Destroy(currentBalloon);
            currentBalloon = null;
        }

        // If still have balloons, spawn the next one
        if (InventoryManager.Instance.GetItemCount(ItemType.Balloon) > 0)
        {
            SpawnBalloonInHand();
        }
    }

    // Optional: if you need to manually clear the hand
    public void ClearHand()
    {
        if (currentBalloon != null)
        {
            Destroy(currentBalloon);
            currentBalloon = null;
        }
    }
}