using UnityEngine;
using System.Collections;

public class PuzzleManager : MonoBehaviour
{
    public static PuzzleManager Instance { get; private set; }

    [Header("Bed Configuration")]
    [SerializeField] private Transform bedTransform;
    [SerializeField] private Transform bedTargetPosition; // Place an empty GameObject where the bed needs to end up
    [SerializeField] private float activationDistance = 0.5f;

    [Header("Pedestals")]
    [SerializeField] private Transform pedestalsParent; // Group your 3 pedestals under 1 empty parent
    [SerializeField] private Vector3 pedestalsRiseOffset = new Vector3(0, 1.5f, 0);
    [SerializeField] private float riseDuration = 3f;

    [Header("Pedestal Sockets")]
    [SerializeField] private OrientationSocket[] pedestals;

    [Header("Final Reward")]
    [SerializeField] private GameObject secretCompartmentDoor;
    [SerializeField] private Renderer fakeShadowRenderer;
    [SerializeField] private float shadowFadeDuration = 2f;
    [SerializeField] private Vector3 doorOpenOffset = new Vector3(0, 1f, 0);

    private bool bedMoved = false;
    private bool puzzleComplete = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Update()
    {
        if (bedMoved || bedTransform == null || bedTargetPosition == null) return;

        float distance = Vector3.Distance(bedTransform.position, bedTargetPosition.position);

        // Print the distance to the Console panel so you can watch it live
        Debug.Log($"Bed Distance to Target: {distance}");

        if (distance <= activationDistance)
        {
            bedMoved = true;
            StartCoroutine(RaisePedestals());
        }
    }

    void Start()
    {
        // Force the shadow to be invisible the moment the game launches
        if (fakeShadowRenderer != null)
        {
            // Use _BaseColor for modern Unity URP shaders
            if (fakeShadowRenderer.material.HasProperty("_BaseColor"))
            {
                Color c = fakeShadowRenderer.material.GetColor("_BaseColor");
                c.a = 0f;
                fakeShadowRenderer.material.SetColor("_BaseColor", c);
            }
            else // Fallback for older Standard shaders
            {
                Color c = fakeShadowRenderer.material.color;
                c.a = 0f;
                fakeShadowRenderer.material.color = c;
            }
        }
    }

    IEnumerator RaisePedestals()
    {
        // Deactivate bed grab interactable so player stops moving it
        if (bedTransform.TryGetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>(out var grab))
        {
            grab.enabled = false;
        }

        Vector3 startPos = pedestalsParent.localPosition;
        Vector3 targetPos = startPos + pedestalsRiseOffset;
        float elapsed = 0f;

        while (elapsed < riseDuration)
        {
            pedestalsParent.localPosition = Vector3.Lerp(startPos, targetPos, elapsed / riseDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        pedestalsParent.localPosition = targetPos;
    }

    // Called by the sockets whenever an object is rotated or placed
    // Called by the sockets whenever an object is rotated or placed
    public void CheckPuzzleState()
    {
        // If the puzzle is already complete, don't do it again
        if (puzzleComplete) return;

        // Loop through all pedestals to see if they are ALL solved
        bool allPedestalsCorrect = true;

        foreach (OrientationSocket pedestal in pedestals)
        {
            if (pedestal != null && !pedestal.IsPuzzleSolved)
            {
                allPedestalsCorrect = false;
                break; // One is wrong, so we don't need to check the rest yet
            }
        }

        if (allPedestalsCorrect)
        {
            TriggerPuzzleComplete();
        }
    }

    private void TriggerPuzzleComplete()
    {
        puzzleComplete = true;
        Debug.Log("ALL PEDESTALS ALIGNED! Hatch door sequence initiated!");

        // Start the automated shadow and hatch sequence
        StartCoroutine(RevealShadowAndOpenHatch());
    }

    IEnumerator RevealShadowAndOpenHatch()
    {
        // 1. Optional UI/Mirror Message
        if (GameManager.Instance != null && GameManager.Instance.mirrorDisplay != null)
        {
            GameManager.Instance.mirrorDisplay.ShowMessage("The alignment is complete. The sun marks the descent.");
        }

        // 2. Fade in the Sun Shadow on top of the Hatch Door
        if (fakeShadowRenderer != null)
        {
            fakeShadowRenderer.gameObject.SetActive(true);
            Material shadowMat = fakeShadowRenderer.material;

            // Handle both URP and Standard shader color properties
            string colorPropertyName = shadowMat.HasProperty("_BaseColor") ? "_BaseColor" : "_Color";

            Color startColor = shadowMat.GetColor(colorPropertyName);
            startColor.a = 0f;
            shadowMat.SetColor(colorPropertyName, startColor);

            float elapsedShadow = 0f;
            while (elapsedShadow < shadowFadeDuration)
            {
                elapsedShadow += Time.deltaTime;
                float alpha = Mathf.Lerp(0f, 0.6f, elapsedShadow / shadowFadeDuration);

                Color c = shadowMat.GetColor(colorPropertyName);
                c.a = alpha;
                shadowMat.SetColor(colorPropertyName, c);

                yield return null;
            }
        }

        // 3. Keep the shadow glowing on the door for a few seconds as requested
        yield return new WaitForSeconds(2.5f);

        // 4. Slide the Hatch Door open automatically
        if (secretCompartmentDoor != null)
        {
            if (secretCompartmentDoor.TryGetComponent<AudioSource>(out var audio))
            {
                audio.Play();
            }

            // --- THE FIX: TURN OFF COLLIDERS BEFORE SLIDING ---
            Collider[] hatchColliders = secretCompartmentDoor.GetComponentsInChildren<Collider>();
            foreach (Collider col in hatchColliders)
            {
                col.enabled = false;
            }

            Vector3 startPos = secretCompartmentDoor.transform.localPosition;
            Vector3 slideOffset = new Vector3(3.0f, 0f, 0f);
            Vector3 targetPos = startPos + slideOffset;

            float elapsedDoor = 0f;
            float slideDuration = 2.0f;

            while (elapsedDoor < slideDuration)
            {
                secretCompartmentDoor.transform.localPosition = Vector3.Lerp(startPos, targetPos, elapsedDoor / slideDuration);
                elapsedDoor += Time.deltaTime;
                yield return null;
            }
            secretCompartmentDoor.transform.localPosition = targetPos;

            Debug.Log("Hatch automatically slid open! Staircase is accessible.");
        }
        else
        {
            Debug.LogError("PuzzleManager Error: Hatch Door (assigned to secretCompartmentDoor) is missing!");
        }
    }}