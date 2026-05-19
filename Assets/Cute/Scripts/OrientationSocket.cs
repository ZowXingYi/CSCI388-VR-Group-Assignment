using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class OrientationSocket : MonoBehaviour
{
    [Header("Target Target Object Setting")]
    [SerializeField] private string targetItemTag; // e.g., "Flower", "Book", "Brush"

    [Header("Correct Target Angle (Y-axis)")]
    [SerializeField] private float targetYRotation = 0f;
    [SerializeField] private float angleTolerance = 25f; // Allows a small margin of user error

    private XRSocketInteractor socket;

    void Start()
    {
        socket = GetComponent<XRSocketInteractor>();

        // Listen to socket events
        socket.selectEntered.AddListener(OnItemPlaced);
        socket.selectExited.AddListener(OnItemRemoved);
    }

    void Update()
    {
        // If an item is resting in the socket, continuously check if the player spins it to the right orientation
        if (socket.hasSelection)
        {
            PuzzleThreeManager.Instance.CheckPuzzleState();
        }
    }

    private void OnItemPlaced(SelectEnterEventArgs args)
    {
        PuzzleThreeManager.Instance.CheckPuzzleState();
    }

    private void OnItemRemoved(SelectExitEventArgs args)
    {
        // Reset state checks if item pulled out
    }

    public bool IsCorrectlyOriented()
    {
        if (!socket.hasSelection) return false;

        // Verify it's the correct object by tag
        UnityEngine.XR.Interaction.Toolkit.Interactables.IXRSelectInteractable interactable = socket.oldestInteractableSelected;
        if (!interactable.transform.CompareTag(targetItemTag)) return false;

        // Calculate angle differences along the Y axis
        float currentY = interactable.transform.localEulerAngles.y;
        float angleDiff = Mathf.Abs(Mathf.DeltaAngle(currentY, targetYRotation));

        return angleDiff <= angleTolerance;
    }
}