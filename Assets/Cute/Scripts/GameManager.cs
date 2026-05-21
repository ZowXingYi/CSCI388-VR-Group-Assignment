using UnityEngine;
using System.Collections;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Bookcases")]
    public Transform leftBookcase;
    public Transform rightBookcase;
    public Vector3 leftSinkOffset = new Vector3(0, -3, 0);
    public Vector3 rightSinkOffset = new Vector3(0, -3, 0);
    public float sinkDuration = 2f;

    [Header("Bookcase Audio")]
    [SerializeField] private AudioSource leftBookcaseAudio;  // Drag left bookcase AudioSource here
    [SerializeField] private AudioSource rightBookcaseAudio; // Drag right bookcase AudioSource here

    [Header("Revealed Objects")]
    public GameObject portraitPainting;    // hidden at start
    public GameObject magnifyingGlassObj; // hidden at start

    [Header("Spotlight")]
    public Light spotlight;

    [Header("Mirror")]
    public MirrorTextDisplay mirrorDisplay;

    private bool leftPulled = false;
    private bool rightPulled = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        portraitPainting.SetActive(false);
        magnifyingGlassObj.SetActive(false);
        spotlight.enabled = false;
    }

    public void OnSconcePulled(bool isLeft)
    {
        if (isLeft && !leftPulled)
        {
            leftPulled = true;
            // Pass the specific AudioSource into the coroutine
            StartCoroutine(SinkBookcase(leftBookcase, leftSinkOffset, leftBookcaseAudio));
            portraitPainting.SetActive(true);
            mirrorDisplay.ShowMessage("You wish to see the truth? Look closely, child...");
        }
        else if (!isLeft && !rightPulled)
        {
            rightPulled = true;
            // Pass the specific AudioSource into the coroutine
            StartCoroutine(SinkBookcase(rightBookcase, rightSinkOffset, rightBookcaseAudio));
            magnifyingGlassObj.SetActive(true);
            mirrorDisplay.ShowMessage("A tiny glass for tiny minds. Read if you can.");
        }

        if (leftPulled && rightPulled)
        {
            spotlight.enabled = true;
            // Ensure the GameObject itself is awake
            spotlight.gameObject.SetActive(true);
            mirrorDisplay.ShowMessage("Let there be light... and let it be your judge.");
        }
    }

    // Updated coroutine to accept and control an AudioSource
    IEnumerator SinkBookcase(Transform bookcase, Vector3 offset, AudioSource movementAudio)
    {
        // 1. Start playing the audio clip immediately
        if (movementAudio != null)
        {
            movementAudio.Play();
        }

        Vector3 startPos = bookcase.localPosition;
        Vector3 targetPos = startPos + offset;
        float elapsed = 0f;

        while (elapsed < sinkDuration)
        {
            bookcase.localPosition = Vector3.Lerp(startPos, targetPos, elapsed / sinkDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        bookcase.localPosition = targetPos;

        // 2. Turn off the audio clip cleanly once movement is finished
        if (movementAudio != null)
        {
            movementAudio.Stop();
        }
    }
}