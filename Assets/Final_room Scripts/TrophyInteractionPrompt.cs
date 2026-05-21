using UnityEngine;
using TMPro;

public class TrophyInteractionPrompt : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Assign your VR Main Camera / Center Eye Anchor here.")]
    public Transform playerCamera;
    [Tooltip("If left empty, this will automatically target the object this script is attached to.")]
    public Transform trophy;
    public GameObject promptUI;

    [Header("Prompt Text")]
    public TMP_Text buttonText;
    public TMP_Text actionText;

    [Header("Settings")]
    public float showDistance = 3f;
    public string buttonLabel = "G";       // Updated default for VR
    public string actionLabel = "Grip";    // Updated default for VR

    [Header("VR Tweaks")]
    [Tooltip("Smoothly rotates the UI to face the VR player's headset.")]
    public bool billboardUI = true;

    private bool isDisabled = false;
    private float sqrShowDistance;

    void Start()
    {
        // Cache squared distance to save performance in Update
        sqrShowDistance = showDistance * showDistance;

        // Auto-assign trophy if attached to the trophy itself
        if (trophy == null)
            trophy = this.transform;

        // Set up initial text values
        if (buttonText != null)
            buttonText.text = buttonLabel;

        if (actionText != null)
            actionText.text = actionLabel;

        // Start with the UI hidden
        if (promptUI != null)
            promptUI.SetActive(false);
    }

    void Update()
    {
        if (isDisabled || playerCamera == null || trophy == null || promptUI == null)
            return;

        // Optimization: Use square magnitude instead of Vector3.Distance
        float sqrDistance = (playerCamera.position - trophy.position).sqrMagnitude;
        bool shouldShow = sqrDistance <= sqrShowDistance;

        // Optimization: Only toggle active state if it actually changed
        if (promptUI.activeSelf != shouldShow)
        {
            promptUI.SetActive(shouldShow);
        }

        // If the UI is visible, make sure it faces the VR headset
        if (shouldShow && billboardUI)
        {
            HandleBillboard();
        }
    }

    private void HandleBillboard()
    {
        // Keeps the UI aligned perfectly flat with the VR camera view plane
        promptUI.transform.rotation = Quaternion.LookRotation(promptUI.transform.position - playerCamera.position);
    }

    public void HidePromptForever()
    {
        isDisabled = true;

        if (promptUI != null)
            promptUI.SetActive(false);
    }
}