// Automatically hide bottle and balloon when the scene starts.
// Automatically SetActive bottle and balloon after click react button and hide them when animation end.

using UnityEngine;

public class ReactionController : MonoBehaviour
{
    [Header("Reaction Objects")]
    public GameObject bottle;
    public GameObject balloon;          // The balloon that inflates during the reaction

    [Header("Result Balloons (3 hidden at start)")]
    [SerializeField] private GameObject[] resultBalloons;   // Assign the 3 balloon objects here

    private void Start()
    {
        // Initial state: hide bottle, reaction balloon, and all result balloons
        if (bottle != null) bottle.SetActive(false);
        if (balloon != null) balloon.SetActive(false);

        foreach (GameObject b in resultBalloons)
        {
            if (b != null) b.SetActive(false);
        }
    }

    public void StartReaction()
    {
        // Show the reaction objects
        if (bottle != null) bottle.SetActive(true);
        if (balloon != null) balloon.SetActive(true);
    }

    public void EndReaction()
    {
        Debug.Log("EndReaction called");
        // Hide the reaction props
        if (bottle != null) bottle.SetActive(false);
        if (balloon != null) balloon.SetActive(false);

        // Clear inventory and add 3 balloons to it
        InventoryManager.Instance.ResetAll();

        // Instead of spawning on the hand, show the 3 pre-placed balloons
        ShowResultBalloons();

        GiveBalloons();
    }

    private void ShowResultBalloons()
    {
        Debug.Log($"ShowResultBalloons running. Array length: {resultBalloons.Length}");
        foreach (GameObject b in resultBalloons)
        {
            Debug.Log("Balloon entry: " + (b != null ? b.name : "NULL"));
            if (b != null) b.SetActive(true);
        }
    }

    private void GiveBalloons()
    {
        // Add 3 balloons, each contributing 1 mole (totalMoles=3, itemCount=3)
        InventoryManager.Instance.AddItem(ItemType.Balloon, 3, 3);
    }
}