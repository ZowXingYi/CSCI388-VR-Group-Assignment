using UnityEngine;
using System.Collections;

public class HatchUnlock : MonoBehaviour
{
    [Header("Key Settings")]
    [SerializeField] private string keyTag = "HatchKey"; // Give your key item this tag in the Inspector
    [SerializeField] private GameObject physicalKeyToHide; // Drag the key mesh here to vanish it on use

    [Header("Animation Settings")]
    [SerializeField] private Transform hatchHingeTransform; // Drag your 'Hinge_HatchDoor' here
    [SerializeField] private Vector3 openRotationAngle = new Vector3(-90f, 0f, 0f); // Flips it upwards/backwards
    [SerializeField] private float openDuration = 2.5f;

    [Header("Audio")]
    [SerializeField] private AudioSource unlockAudio;

    private bool isHatchOpen = false;

    private void OnTriggerEnter(Collider other)
    {
        // If the hatch is already open or the object touching it isn't the key, do nothing
        if (isHatchOpen || !other.CompareTag(keyTag)) return;

        isHatchOpen = true;
        StartCoroutine(OpenHatchSequence());
    }

    IEnumerator OpenHatchSequence()
    {
        if (unlockAudio != null) unlockAudio.Play();

        // 1. Disable the key's visual and physics so it looks like it was inserted/used
        if (physicalKeyToHide != null)
        {
            physicalKeyToHide.SetActive(false);
        }

        // 2. Smoothly rotate the hatch hinge open
        Quaternion startRot = hatchHingeTransform.localRotation;
        Quaternion targetRot = startRot * Quaternion.Euler(openRotationAngle);
        float elapsed = 0f;

        while (elapsed < openDuration)
        {
            elapsed += Time.deltaTime;
            hatchHingeTransform.localRotation = Quaternion.Slerp(startRot, targetRot, elapsed / openDuration);
            yield return null;
        }

        hatchHingeTransform.localRotation = targetRot;

        // 3. Turn off the hatch's collider entirely so the player can safely walk down
        if (hatchHingeTransform.TryGetComponent<Collider>(out var col)) col.enabled = false;

        // Also turn off colliders on child objects just in case
        Collider[] childColliders = hatchHingeTransform.GetComponentsInChildren<Collider>();
        foreach (Collider childCol in childColliders) childCol.enabled = false;

        Debug.Log("Hatch fully opened! Path to staircase is clear.");
    }
}