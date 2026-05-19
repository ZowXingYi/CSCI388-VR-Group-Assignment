// Automatically hide bottle and balloon when the scene starts.
// Automatically SetActive bottle and balloon after click react button and hide them when animation end.

using UnityEngine;

public class ReactionController : MonoBehaviour
{
    [Header("Reaction Objects")]
    public GameObject bottle;
    public GameObject balloon;

    private void Start()
    {
        // === INITIAL STATE: Hide both at the beginning ===
        if (bottle != null) bottle.SetActive(false);
        if (balloon != null) balloon.SetActive(false);
    }

    public void StartReaction()
    {
        // Show both objects
        if (bottle != null) bottle.SetActive(true);
        if (balloon != null) balloon.SetActive(true);
    }

    public void EndReaction()
    {
        // Hide both objects after animation ends
        if (bottle != null) bottle.SetActive(false);
        if (balloon != null) balloon.SetActive(false);
    }
}