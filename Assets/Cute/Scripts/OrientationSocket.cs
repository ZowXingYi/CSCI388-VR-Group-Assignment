using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class OrientationSocket : MonoBehaviour
{
    [Header("Target Settings")]
    [SerializeField] private string targetItemTag;
    [SerializeField] private float targetYRotation = 0f;
    [SerializeField] private float angleTolerance = 15f; // Tighter tolerance since UI is precise!

    [Header("UI & Feedback Elements")]
    [SerializeField] private GameObject interactionButtonCanvas; // Floating World Space Button
    [SerializeField] private Light pedestalLight;                // The point/spot light for feedback
    [SerializeField] private AudioSource victoryAudio;           // Success sound effect
    [SerializeField] private bool invertRotation = false;

    private XRSocketInteractor socket;
    private IXRInteractable currentItem;
    public bool IsPuzzleSolved { get; private set; } = false;
    void Start()
    {
        socket = GetComponent<XRSocketInteractor>();

        // Setup XRI 3.x Listeners
        socket.selectEntered.AddListener(OnItemPlaced);
        socket.selectExited.AddListener(OnItemRemoved);

        if (interactionButtonCanvas != null) interactionButtonCanvas.SetActive(false);
    }

    private void OnItemPlaced(SelectEnterEventArgs args)
    {
        Debug.Log("SOCKET: Item detected! Checking tag...");
        if (IsPuzzleSolved) return;

        // Keep track of the current item inside the socket
        if (socket.interactablesSelected != null && socket.interactablesSelected.Count > 0)
        {
            currentItem = socket.interactablesSelected[0];

            // Only show the UI button if it's the correct item for this specific pedestal
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

    // This public method will be called directly by your UI Buttons!
    public void RotateCurrentItem(float angleAmount)
    {
        IXRInteractable foundItem = null;
        if (socket.interactablesSelected != null && socket.interactablesSelected.Count > 0)
        {
            foundItem = socket.interactablesSelected[0];
        }

        if (foundItem == null || IsPuzzleSolved) return;

        // Determine if we need to spin backwards for this specific pedestal
        float directionMultiplier = invertRotation ? -1f : 1f;
        float finalAngle = angleAmount * directionMultiplier;

        if (socket.attachTransform != null)
        {
            socket.attachTransform.Rotate(0f, finalAngle, 0f, Space.Self);

            // Force the mesh to update its transform position
            foundItem.transform.position = socket.attachTransform.position;
            foundItem.transform.rotation = socket.attachTransform.rotation;
        }
        else
        {
            foundItem.transform.Rotate(0f, finalAngle, 0f, Space.Self);
        }

        CheckRotationValidity();
    }

    private void CheckRotationValidity()
    {
        // Safety check
        if (socket.interactablesSelected == null || socket.interactablesSelected.Count == 0) return;

        // 1. Get the actual physical object sitting in the socket right now
        Transform physicalItem = socket.interactablesSelected[0].transform;

        // 2. Read its absolute World Y rotation (ignores parent/anchor confusion)
        float currentWorldY = physicalItem.eulerAngles.y;

        // 3. Calculate the absolute difference between the item's world angle and your target
        float angleDiff = Mathf.Abs(Mathf.DeltaAngle(currentWorldY, targetYRotation));

        Debug.Log($"Item: {physicalItem.name} | World Y: {currentWorldY:F1} | Target: {targetYRotation} | Diff: {angleDiff:F1}");

        // 4. Strict check: Tolerance must be small (e.g., 10-15 degrees)
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

        // --- LOCK DOWN FOR COMPONENT DEPENDENCIES ---
        if (socket.interactablesSelected != null && socket.interactablesSelected.Count > 0)
        {
            Transform physicalItem = socket.interactablesSelected[0].transform;

            // 1. Permanent parenting
            physicalItem.SetParent(transform, true);

            // 2. Destroy the Grab Interactable so XRI completely forgets about this item
            XRGrabInteractable grab = physicalItem.GetComponent<XRGrabInteractable>();
            if (grab != null)
            {
                Destroy(grab);
            }

            // 3. Lock the Rigidbody safely
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

        // 4. Disable the socket component safely
        socket.enabled = false;

        // 5. Notify your global manager
        PuzzleManager.Instance.CheckPuzzleState();
    }
}