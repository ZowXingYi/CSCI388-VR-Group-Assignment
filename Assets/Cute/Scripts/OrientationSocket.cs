using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class OrientationSocket : MonoBehaviour
{
    // Create an enum to choose the rotation behavior in the Inspector
    public enum RotationAxis { Y_Axis_Horizontal, X_Axis_Vertical }

    [Header("Target Settings")]
    [SerializeField] private string targetItemTag;
    [SerializeField] private RotationAxis rotationAxis = RotationAxis.Y_Axis_Horizontal; // Default to normal horizontal spin
    [SerializeField] private float targetRotationAngle = 0f; // Renamed to keep it generic for X or Y
    [SerializeField] private float angleTolerance = 15f;
    [SerializeField] private bool invertRotation = false;

    [Header("UI & Feedback Elements")]
    [SerializeField] private GameObject interactionButtonCanvas;
    [SerializeField] private Light pedestalLight;
    [SerializeField] private AudioSource victoryAudio;

    private XRSocketInteractor socket;
    private IXRInteractable currentItem;
    public bool IsPuzzleSolved { get; private set; } = false;

    void Start()
    {
        socket = GetComponent<XRSocketInteractor>();
        socket.selectEntered.AddListener(OnItemPlaced);
        socket.selectExited.AddListener(OnItemRemoved);

        if (interactionButtonCanvas != null) interactionButtonCanvas.SetActive(false);
    }

    private void OnItemPlaced(SelectEnterEventArgs args)
    {
        if (IsPuzzleSolved) return;

        if (socket.interactablesSelected != null && socket.interactablesSelected.Count > 0)
        {
            currentItem = socket.interactablesSelected[0];

            if (currentItem.transform.CompareTag(targetItemTag))
            {
                if (interactionButtonCanvas != null) interactionButtonCanvas.SetActive(true);
            }
        }
    }

    private void OnItemRemoved(SelectExitEventArgs args)
    {
        currentItem = null;
        if (interactionButtonCanvas != null) interactionButtonCanvas.SetActive(false);
    }

    public void RotateCurrentItem(float angleAmount)
    {
        IXRInteractable foundItem = null;
        if (socket.interactablesSelected != null && socket.interactablesSelected.Count > 0)
        {
            foundItem = socket.interactablesSelected[0];
        }

        if (foundItem == null || IsPuzzleSolved) return;

        float directionMultiplier = invertRotation ? -1f : 1f;
        float finalAngle = angleAmount * directionMultiplier;

        if (socket.attachTransform != null)
        {
            // Choose axis based on Inspector configuration
            if (rotationAxis == RotationAxis.X_Axis_Vertical)
            {
                // Rotate Forward/Backward along the X axis
                socket.attachTransform.Rotate(finalAngle, 0f, 0f, Space.Self);
            }
            else
            {
                // Normal Left/Right spin along the Y axis
                socket.attachTransform.Rotate(0f, finalAngle, 0f, Space.Self);
            }

            foundItem.transform.position = socket.attachTransform.position;
            foundItem.transform.rotation = socket.attachTransform.rotation;
        }
        else
        {
            if (rotationAxis == RotationAxis.X_Axis_Vertical)
            {
                foundItem.transform.Rotate(finalAngle, 0f, 0f, Space.Self);
            }
            else
            {
                foundItem.transform.Rotate(0f, finalAngle, 0f, Space.Self);
            }
        }

        CheckRotationValidity();
    }

    private void CheckRotationValidity()
    {
        if (socket.interactablesSelected == null || socket.interactablesSelected.Count == 0) return;

        Transform physicalItem = socket.interactablesSelected[0].transform;
        float currentAngle = 0f;

        if (rotationAxis == RotationAxis.X_Axis_Vertical)
        {
            currentAngle = physicalItem.eulerAngles.x;
        }
        else
        {
            currentAngle = physicalItem.eulerAngles.y;
        }

        // Use DeltaAngle to handle the 0/360 wrap-around perfectly
        float angleDiff = Mathf.Abs(Mathf.DeltaAngle(currentAngle, targetRotationAngle));

        Debug.Log($"Item: {physicalItem.name} | Mode: {rotationAxis} | Current Angle: {currentAngle:F1} | Target: {targetRotationAngle} | Diff: {angleDiff:F1}");

        // Strict validation check
        if (angleDiff <= angleTolerance)
        {
            TriggerSuccessFeedback();
        }
    }

    private void TriggerSuccessFeedback()
    {
        IsPuzzleSolved = true;

        if (interactionButtonCanvas != null) interactionButtonCanvas.SetActive(false);

        if (pedestalLight != null)
        {
            pedestalLight.enabled = true;
            pedestalLight.color = Color.green;
            pedestalLight.intensity = 35f;
        }

        if (victoryAudio != null)
        {
            victoryAudio.Play();
        }

        // --- COMPONENT HAND-OFF FIXED WITH EXPLICIT CASTING ---
        if (socket.interactablesSelected != null && socket.interactablesSelected.Count > 0)
        {
            IXRInteractable interactableItem = socket.interactablesSelected[0];
            Transform physicalItem = interactableItem.transform;

            // Try casting the interface to the concrete XRGrabInteractable to access its attachTransform safely
            XRGrabInteractable grabItem = interactableItem as XRGrabInteractable;

            if (socket.attachTransform != null)
            {
                // Check if our cast succeeded and if the item has a custom attach transform
                if (grabItem != null && grabItem.attachTransform != null && grabItem.attachTransform != physicalItem)
                {
                    Transform itemAttach = grabItem.attachTransform;

                    // Calculate local offset from the item's origin to its attach point
                    Vector3 localOffset = physicalItem.position - itemAttach.position;

                    // Parent it first
                    physicalItem.SetParent(transform, true);

                    // Snap it perfectly matching the socket anchor plus the offset
                    physicalItem.position = socket.attachTransform.position + localOffset;
                    physicalItem.rotation = socket.attachTransform.rotation * Quaternion.Inverse(itemAttach.localRotation);
                }
                else
                {
                    // Fallback if there's no custom item attach transform
                    physicalItem.SetParent(transform, true);
                    physicalItem.position = socket.attachTransform.position;
                    physicalItem.rotation = socket.attachTransform.rotation;
                }
            }
            else
            {
                physicalItem.SetParent(transform, true);
            }

            // 2. Destroy the Grab Interactable safely using our reference
            if (grabItem != null)
            {
                Destroy(grabItem);
            }
            else
            {
                // Just in case it was a different type of interactable
                XRGrabInteractable fallbackGrab = physicalItem.GetComponent<XRGrabInteractable>();
                if (fallbackGrab != null) Destroy(fallbackGrab);
            }

            // 3. Freeze Rigidbody
            Rigidbody rb = physicalItem.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true;
                rb.useGravity = false;
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }

            // 4. Disable colliders
            Collider[] itemColliders = physicalItem.GetComponentsInChildren<Collider>();
            foreach (Collider col in itemColliders)
            {
                col.enabled = false;
            }
        }

        socket.enabled = false;
        PuzzleManager.Instance.CheckPuzzleState();
    }
}