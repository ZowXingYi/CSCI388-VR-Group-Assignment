using UnityEngine;

public class VRSconce : MonoBehaviour
{
    [Header("Sconce Configuration")]
    [SerializeField] private bool isLeftSconce;
    [SerializeField] private Transform handleTransform; // The moving pivot part of your sconce model

    [Header("Animation Settings")]
    [SerializeField] private float pullRotationX = 35f; // Degrees to rotate downward
    [SerializeField] private float pullSpeed = 6f;

    private bool isPulled = false;
    private Quaternion targetRotation;

    void Start()
    {
        // Default to this object if a specific handle transform wasn't assigned
        if (handleTransform == null)
            handleTransform = transform;

        targetRotation = handleTransform.localRotation;
    }

    void Update()
    {
        // Smoothly animate the pull action once triggered
        if (isPulled)
        {
            handleTransform.localRotation = Quaternion.Slerp(
                handleTransform.localRotation,
                targetRotation,
                Time.deltaTime * pullSpeed
            );
        }
    }

    /// <summary>
    /// This method is called by the XR Interactable component when the player pulls the sconce.
    /// </summary>
    public void PullSconce()
    {
        if (isPulled) return; // Prevent pulling multiple times
        isPulled = true;

        // Changing the multiplication order forces Unity to calculate 
        // the rotation relative to the Hinge's own freshly aligned local X-axis
        targetRotation = handleTransform.localRotation * Quaternion.Euler(pullRotationX, 0, 0);

        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnSconcePulled(isLeftSconce);
        }
    }
}