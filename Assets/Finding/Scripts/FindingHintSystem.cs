using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class FindingHintSystem : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject hintPanel;
    [SerializeField] private TMP_Text hintText;
    [SerializeField] private Button closeButton;
    [SerializeField] private Button nextButton;

    [Header("Positioning Settings")]
    [SerializeField] private Transform playerHead;           // Usually Main Camera or XR Rig Head
    [SerializeField] private float distanceInFront = 100.0f;   // How far in front of player
    [SerializeField] private float heightOffset = 0.1f;      // Slight vertical adjustment
    [SerializeField] private bool facePlayer = true;         // Rotate to face player

    [Header("Glow System")]
    [SerializeField] private GameObject[] collectableObjects;   // Drag all Al cans, NaOH, H2O here
    [SerializeField] private Material glowMaterial;

    private int currentHintIndex = -1;

    private readonly string[] hints = new string[]
    {
        "Puzzle 1: Collect the shining objects that are visible even through walls.\n\nLook for glowing Al cans, NaOH bottles, and H2O flasks.",

        "Puzzle 2: Check the blackboard for the chemical equation.\n\nYou need exactly:\n• 2 Al\n• 2 NaOH\n• 6 H2O",

        "Puzzle 3: After the reaction, grab the balloons and place them on the door sockets beside the big green balloon."
    };

    private Material[] originalMaterials;
    private bool isGlowing = false;

    private void Awake()
    {
        if (collectableObjects != null && collectableObjects.Length > 0)
        {
            originalMaterials = new Material[collectableObjects.Length];

            for (int i = 0; i < collectableObjects.Length; i++)
            {
                if (collectableObjects[i] != null)
                {
                    var renderer = collectableObjects[i].GetComponent<Renderer>();
                    if (renderer != null)
                        originalMaterials[i] = renderer.material;
                }
            }
        }
    }

    private void Start()
    {
        if (hintPanel != null)
            hintPanel.SetActive(false);

        // Auto-find player head if not assigned
        if (playerHead == null)
        {
            playerHead = Camera.main?.transform;
            if (playerHead == null)
                Debug.LogWarning("Player Head not assigned and Camera.main not found!");
        }

        // Setup buttons
        if (nextButton != null)
            nextButton.onClick.AddListener(ShowNextHint);

        if (closeButton != null)
            closeButton.onClick.AddListener(HideHint);
    }

    public void ShowNextHint()
    {
        currentHintIndex = (currentHintIndex + 1) % hints.Length;
        PositionInFrontOfPlayer();

        if (hintPanel != null)
        {
            hintPanel.SetActive(true);
            hintText.text = hints[currentHintIndex];
        }

        EnableGlow(true);   // ← Turn on X-ray glow
    }

    public void HideHint()
    {
        if (hintPanel != null)
            hintPanel.SetActive(false);

        EnableGlow(false);  // ← Turn off
    }

    private void EnableGlow(bool enable)
    {
        if (collectableObjects == null || glowMaterial == null) return;

        for (int i = 0; i < collectableObjects.Length; i++)
        {
            if (collectableObjects[i] == null) continue;

            var renderer = collectableObjects[i].GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material = enable ? glowMaterial : originalMaterials[i];
            }
        }

        isGlowing = enable;
    }

    private void PositionInFrontOfPlayer()
    {
        if (playerHead == null || hintPanel == null) return;

        // Calculate position in front of player
        Vector3 forward = playerHead.forward;
        forward.y = 0;                    // Keep it horizontal
        forward.Normalize();

        Vector3 targetPosition = playerHead.position + forward * distanceInFront;
        targetPosition.y += heightOffset;

        hintPanel.transform.position = targetPosition;

        // Make the panel face the player
        if (facePlayer)
        {
            hintPanel.transform.LookAt(playerHead.position);
            hintPanel.transform.Rotate(0, 180, 0); // Fix the rotation (canvas faces wrong way by default)
        }
    }

    // Optional: Show specific hint
    public void ShowHint(int index)
    {
        if (index >= 0 && index < hints.Length)
        {
            currentHintIndex = index;
            PositionInFrontOfPlayer();

            if (hintPanel != null)
            {
                hintPanel.SetActive(true);
                hintText.text = hints[index];
            }

            EnableGlow(true);
        }
    }

    public void ResetHints()
    {
        currentHintIndex = -1;
        EnableGlow(false);
        HideHint();
    }
}