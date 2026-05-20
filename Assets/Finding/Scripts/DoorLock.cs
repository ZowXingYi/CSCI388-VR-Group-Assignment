using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class DoorLock : MonoBehaviour
{
    [SerializeField] private XRSocketInteractor[] balloonSockets;
    [SerializeField] private GameObject newDoor;

    [SerializeField] private UnityEvent onAllBalloonsPlaced; // kept if you still want external hooks

    private int balloonsPlaced = 0;

    private void Start()
    {
        if (newDoor != null)
            newDoor.SetActive(false); // ensure it starts hidden

        foreach (var socket in balloonSockets)
            socket.selectEntered.AddListener(OnBalloonPlaced);
    }

    private void OnBalloonPlaced(SelectEnterEventArgs args)
    {
        balloonsPlaced++;
        if (balloonsPlaced >= balloonSockets.Length)
        {
            // 1. Disable this door
            gameObject.SetActive(false);

            // 2. Activate the new door and play its animation
            if (newDoor != null)
            {
                newDoor.SetActive(true);
            }

            // 3. Fire any additional inspector events
            onAllBalloonsPlaced?.Invoke();

            // Disable sockets to prevent further interaction
            foreach (var socket in balloonSockets)
                socket.enabled = false;
        }
    }
}