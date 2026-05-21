using UnityEngine;
using System.Collections;

public class RandomHorrorSound : MonoBehaviour
{
    public AudioSource audioSource;

    public float minTime = 30f;
    public float maxTime = 90f;

    void Start()
    {
        StartCoroutine(PlayRandomSound());
    }

    IEnumerator PlayRandomSound()
    {
        while (true)
        {
            float waitTime =
                Random.Range(minTime, maxTime);

            yield return new WaitForSeconds(waitTime);

            audioSource.Play();
        }
    }
}