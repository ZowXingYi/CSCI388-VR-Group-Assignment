using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class ResetOnDrop : MonoBehaviour
{
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private XRGrabInteractable grabInteractable;

    void Start()
    {
        originalPosition = transform.position;
        originalRotation = transform.rotation;
        grabInteractable = GetComponent<XRGrabInteractable>();
        grabInteractable.selectExited.AddListener(OnDropped);
    }

    private void OnDropped(SelectExitEventArgs args)
    {
        StartCoroutine(CheckAndReset());
    }

    private IEnumerator CheckAndReset()
    {
        yield return new WaitForSeconds(2f); // Give it 2 seconds on the floor
        // Reset if not held
        if (!grabInteractable.isSelected)
        {
            transform.position = originalPosition;
            transform.rotation = originalRotation;
            GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
            GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        }
    }
}