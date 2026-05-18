// Attached to the Balloon inventory slot’s button.
// When pressed, spawns a balloon prefab at the player’s hand and consumes one from inventory.

using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class BalloonSpawner : MonoBehaviour
{
    [SerializeField] private GameObject balloonPrefab;
    [SerializeField] private Transform handAttachPoint;   // e.g., the right hand controller

    public void SpawnBalloon()
    {
        if (InventoryManager.Instance.GetCount(ItemType.Balloon) <= 0)
            return;

        InventoryManager.Instance.RemoveItem(ItemType.Balloon, 1);

        GameObject balloon = Instantiate(balloonPrefab, handAttachPoint.position, handAttachPoint.rotation);
        // Attach to hand using XR Grab Interactable
        XRGrabInteractable grab = balloon.GetComponent<XRGrabInteractable>();
        if (grab != null)
        {
            // Simulate a grab: normally you'd use interaction manager, but for quick demo we parent it
            balloon.transform.SetParent(handAttachPoint);
            // You may want to use XR Interaction Toolkit's attach system properly.
        }
    }
}