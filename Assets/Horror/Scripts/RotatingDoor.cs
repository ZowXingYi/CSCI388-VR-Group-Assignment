using UnityEngine;

public class RotatingDoor : MonoBehaviour
{
    public bool isOpen = false;

    public float speed = 2f;

    public Vector3 closedRotation;
    public Vector3 openRotation;

    private Quaternion targetRotation;

    void Start()
    {
        targetRotation = Quaternion.Euler(closedRotation);
    }

    void Update()
    {
        transform.localRotation =
            Quaternion.Lerp(
                transform.localRotation,
                targetRotation,
                Time.deltaTime * speed
            );
    }

    public void OpenDoor()
    {
        isOpen = true;
        targetRotation = Quaternion.Euler(openRotation);

        Debug.Log("Door Opened");
    }

    public void CloseDoor()
    {
        isOpen = false;
        targetRotation = Quaternion.Euler(closedRotation);

        Debug.Log("Door Closed");
    }

    public void ToggleDoor()
    {
        isOpen = !isOpen;

        if (isOpen)
        {
            targetRotation = Quaternion.Euler(openRotation);
        }
        else
        {
            targetRotation = Quaternion.Euler(closedRotation);
        }
    }
}