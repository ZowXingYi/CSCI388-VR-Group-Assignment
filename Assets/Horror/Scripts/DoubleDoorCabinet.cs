using UnityEngine;

public class DoubleDoorCabinet : MonoBehaviour
{
    [Header("Doors")]
    public Transform leftDoor;
    public Transform rightDoor;

    [Header("Settings")]
    public float speed = 2f;

    [Header("Open Angles")]
    public float leftOpenAngle = 90f;
    public float rightOpenAngle = -90f;

    private bool leftOpen = false;
    private bool rightOpen = false;

    private Quaternion leftClosedRot;
    private Quaternion leftOpenRot;

    private Quaternion rightClosedRot;
    private Quaternion rightOpenRot;

    void Start()
    {
        // Save original rotations
        leftClosedRot = leftDoor.localRotation;
        rightClosedRot = rightDoor.localRotation;

        // Calculate open rotations
        leftOpenRot =
            Quaternion.Euler(
                leftDoor.localEulerAngles +
                new Vector3(0, leftOpenAngle, 0)
            );

        rightOpenRot =
            Quaternion.Euler(
                rightDoor.localEulerAngles +
                new Vector3(0, rightOpenAngle, 0)
            );
    }

    void Update()
    {
        // LEFT DOOR
        Quaternion targetLeftRot =
            leftOpen ? leftOpenRot : leftClosedRot;

        leftDoor.localRotation =
            Quaternion.Lerp(
                leftDoor.localRotation,
                targetLeftRot,
                Time.deltaTime * speed
            );

        // RIGHT DOOR
        Quaternion targetRightRot =
            rightOpen ? rightOpenRot : rightClosedRot;

        rightDoor.localRotation =
            Quaternion.Lerp(
                rightDoor.localRotation,
                targetRightRot,
                Time.deltaTime * speed
            );
    }

    // Toggle LEFT door
    public void ToggleLeftDoor()
    {
        leftOpen = !leftOpen;

        Debug.Log("Left Door Triggered");
    }

    // Toggle RIGHT door
    public void ToggleRightDoor()
    {
        rightOpen = !rightOpen;

        Debug.Log("Right Door Triggered");
    }
}