using UnityEngine;

public class FridgeDoor : MonoBehaviour
{
    public Transform upperDoor;
    public Transform lowerDoor;

    public bool upperOpen = false;
    public bool lowerOpen = false;

    public float speed = 2f;

    private Quaternion upperClosedRot;
    private Quaternion upperOpenRot;

    private Quaternion lowerClosedRot;
    private Quaternion lowerOpenRot;

    void Start()
    {
        upperClosedRot = upperDoor.localRotation;
        lowerClosedRot = lowerDoor.localRotation;

        upperOpenRot =
            Quaternion.Euler(
                upperDoor.localEulerAngles + new Vector3(0, -90, 0)
            );

        lowerOpenRot =
            Quaternion.Euler(
                lowerDoor.localEulerAngles + new Vector3(0, -90, 0)
            );
        Debug.Log(lowerOpenRot);
    }

    void Update()
    {
        // Upper Door
        if (upperOpen)
        {
            upperDoor.localRotation =
                Quaternion.Lerp(
                    upperDoor.localRotation,
                    upperOpenRot,
                    Time.deltaTime * speed
                );
        }
        else
        {
            upperDoor.localRotation =
                Quaternion.Lerp(
                    upperDoor.localRotation,
                    upperClosedRot,
                    Time.deltaTime * speed
                );
        }

        // Lower Door
        if (lowerOpen)
        {
            lowerDoor.localRotation =
                Quaternion.Lerp(
                    lowerDoor.localRotation,
                    lowerOpenRot,
                    Time.deltaTime * speed
                );
        }
        else
        {
            lowerDoor.localRotation =
                Quaternion.Lerp(
                    lowerDoor.localRotation,
                    lowerClosedRot,
                    Time.deltaTime * speed
                );
        }
    }

    public void ToggleUpperDoor()
    {
        upperOpen = !upperOpen;
        Debug.Log("Upper Door Triggered");
    }

    public void ToggleLowerDoor()
    {
        lowerOpen = !lowerOpen;
        Debug.Log("Lower Door Triggered");
    }
}