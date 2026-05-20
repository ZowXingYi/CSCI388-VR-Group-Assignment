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
    public void CheckPuzzleState()
    {
        // Loop through all pedestals to see if they are ALL solved
        bool allPedestalsCorrect = true;

        foreach (OrientationSocket pedestal in pedestals)
        {
            // We now check the public 'isPuzzleSolved' boolean variable 
            // that gets set to true automatically when the player rotates it via UI!
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
        Debug.Log("ALL PEDESTALS ALIGNED! Final room progression unlocked!");
        // Your existing room completion code goes here (e.g., opening the final door)
    }

    IEnumerator OpenSecretCompartment()
    {
        if (GameManager.Instance != null && GameManager.Instance.mirrorDisplay != null)
        {
            GameManager.Instance.mirrorDisplay.ShowMessage("The walls crack... freedom beckons.");
        }

        Vector3 startPos = secretCompartmentDoor.transform.localPosition;
        Vector3 targetPos = startPos + doorOpenOffset;
        float elapsed = 0f;

        while (elapsed < 1.5f)
        {
            secretCompartmentDoor.transform.localPosition = Vector3.Lerp(startPos, targetPos, elapsed / 1.5f);
            elapsed += Time.deltaTime;
            yield return null;
        }
        secretCompartmentDoor.transform.localPosition = targetPos;
    }
}