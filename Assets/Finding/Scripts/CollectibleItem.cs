// Attach to any collectable object (Al can, NaOH bottle, H2O flask) along with an XR Grab Interactable.
// When grabbed, it adds itself to the inventory and is destroyed.

using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRGrabInteractable))]
public class CollectibleItem : MonoBehaviour
{
    [SerializeField] private ItemType itemType;
    [SerializeField] private int moleAmount = 1;

    private XRGrabInteractable grabInteractable;

    private void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        grabInteractable.selectEntered.AddListener(OnGrabbed);
    }

    private void OnDestroy()
    {
        if (grabInteractable != null)
            grabInteractable.selectEntered.RemoveListener(OnGrabbed);
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        if (InventoryManager.Instance == null)
        {
            Debug.LogError("InventoryManager.Instance is null!", this);
            return;
        }

        // Adds 1 object, with the defined mole amount
        InventoryManager.Instance.AddItem(itemType, moleAmount, 1);
        Destroy(gameObject);
    }
}