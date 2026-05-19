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
            StartCoroutine(SinkBookcase(leftBookcase, leftSinkOffset));
            portraitPainting.SetActive(true);
            mirrorDisplay.ShowMessage("You wish to see the truth? Look closely, child...");
        }
        else if (!isLeft && !rightPulled)
        {
            rightPulled = true;
            StartCoroutine(SinkBookcase(rightBookcase, rightSinkOffset));
            magnifyingGlassObj.SetActive(true);
            mirrorDisplay.ShowMessage("A tiny glass for tiny minds. Read if you can.");
        }

        if (leftPulled && rightPulled)
        {
            spotlight.enabled = true;
            mirrorDisplay.ShowMessage("Let there be light... and let it be your judge.");
        }
    }

    IEnumerator SinkBookcase(Transform bookcase, Vector3 offset)
    {
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
    }
}