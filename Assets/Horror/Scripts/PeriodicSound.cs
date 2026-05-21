using UnityEngine;

public class PeriodicSound : MonoBehaviour
{
    public AudioSource audioSource;

    public float interval = 60f;

    void Start()
    {
        InvokeRepeating(
            nameof(PlaySound),
            interval,
            interval
        );
    }

    void PlaySound()
    {
        audioSource.Play();
    }
}