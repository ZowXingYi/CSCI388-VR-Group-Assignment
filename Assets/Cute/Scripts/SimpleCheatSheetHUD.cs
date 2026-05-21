using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SimpleCheatSheetHUD : MonoBehaviour
{
    [Header("Main Toggle Controls")]
    [SerializeField] private GameObject slideshowGroup; // Drag a parent object containing the image & arrow buttons here
    [SerializeField] private Button mainClueButton;      // Drag your corner "? Clues" button here
    [SerializeField] private TextMeshProUGUI mainButtonText;

    [Header("Image Display")]
    [SerializeField] private Image displayImageComponent; // Drag your 'CheatSheet_ImageDisplay' component here
    [SerializeField] private Sprite[] cheatSheetPages;     // Lock your downloaded PNG array here!

    [Header("Navigation Buttons")]
    [SerializeField] private Button nextButton;
    [SerializeField] private Button prevButton;
    [SerializeField] private TextMeshProUGUI pageCounterText; // Optional: A text element showing "1 / 3"

    private int currentPageIndex = 0;
    private bool isSlideshowVisible = false;

    void Start()
    {
        // 1. Hide the slideshow elements at launch
        if (slideshowGroup != null) slideshowGroup.SetActive(false);

        // 2. Assign Button Event Listeners
        if (mainClueButton != null) mainClueButton.onClick.AddListener(ToggleSlideshow);
        if (nextButton != null) nextButton.onClick.AddListener(ShowNextPage);
        if (prevButton != null) prevButton.onClick.AddListener(ShowPreviousPage);

        UpdateSlideshowUI();
    }

    private void ToggleSlideshow()
    {
        isSlideshowVisible = !isSlideshowVisible;

        if (slideshowGroup != null)
        {
            slideshowGroup.SetActive(isSlideshowVisible);
        }

        UpdateSlideshowUI();
    }

    private void ShowNextPage()
    {
        if (cheatSheetPages.Length == 0) return;

        // Advance index and loop back to 0 if it exceeds the list capacity
        currentPageIndex = (currentPageIndex + 1) % cheatSheetPages.Length;
        UpdateSlideshowUI();
    }

    private void ShowPreviousPage()
    {
        if (cheatSheetPages.Length == 0) return;

        // Regress index and wrap to the end of the array if falling below zero
        currentPageIndex--;
        if (currentPageIndex < 0)
        {
            currentPageIndex = cheatSheetPages.Length - 1;
        }
        UpdateSlideshowUI();
    }

    private void UpdateSlideshowUI()
    {
        // Update main corner button text string
        if (mainButtonText != null)
        {
            mainButtonText.text = isSlideshowVisible ? "Close Clues" : "? Clues";
        }

        // Swap the active sprite asset on screen if array contains elements
        if (cheatSheetPages.Length > 0 && displayImageComponent != null)
        {
            displayImageComponent.sprite = cheatSheetPages[currentPageIndex];
        }

        // Optional: Dynamically update page tracking numbers (e.g., "Page: 1 / 3")
        if (pageCounterText != null && cheatSheetPages.Length > 0)
        {
            pageCounterText.text = $"{currentPageIndex + 1} / {cheatSheetPages.Length}";
        }
    }
}