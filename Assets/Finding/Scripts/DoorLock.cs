// Manages the three balloon snap points. When all are filled, triggers explosion animation.

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class DoorLock : MonoBehaviour
{
    [SerializeField] private XRSocketInteractor[] balloonSockets; // 3 sockets on the door
    [SerializeField] private UnityEvent onAllBalloonsPlaced;

    private int balloonsPlaced = 0;

    private void Start()
    {
        foreach (var socket in balloonSockets)
        {
            socket.selectEntered.AddListener(OnBalloonPlaced);
        }
    }

    private void OnBalloonPlaced(SelectEnterEventArgs args)
    {
        balloonsPlaced++;
        if (balloonsPlaced >= balloonSockets.Length)
        {
            onAllBalloonsPlaced?.Invoke();
            // Disable further interaction
            foreach (var socket in balloonSockets)
                socket.enabled = false;
        }
    }
}