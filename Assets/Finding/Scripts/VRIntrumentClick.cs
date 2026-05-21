using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

/// <summary>
/// Makes the lab station react to a VR pointer click (select).
/// Place this on the instrument object alongside a Collider.
/// </summary>
[RequireComponent(typeof(Collider))]
public class VRInstrumentClick : XRSimpleInteractable
{
    [SerializeField] private AnalyticalLabStation labStation;

    protected override void Awake()
    {
        base.Awake();
        // If not assigned, try to get the station from the same GameObject
        if (labStation == null)
            labStation = GetComponent<AnalyticalLabStation>();
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);
        labStation.ToggleLabPanel();
    }
}