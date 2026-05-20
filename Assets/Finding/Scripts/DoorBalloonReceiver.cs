using UnityEngine;

public class DoorBalloonReceiver : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Check if it's the player's hand balloon (you can use a tag or reference)
        if (other.CompareTag("Balloon"))
        {
            BalloonSpawner spawner = FindObjectOfType<BalloonSpawner>();
            if (spawner != null)
            {
                spawner.ConsumeBalloonAndRespawn();
            }
        }
    }
}