// Manages the lab area trigger and the reaction logic.
// It shows/hides a UI panel with sliders and handles the reaction.

using UnityEngine;
using UnityEngine.Events;

public class AnalyticalLabStation : MonoBehaviour
{
    // Static events for other systems (e.g., audio, UI, tutorial)
    public static event System.Action OnLabEnter;
    public static event System.Action OnLabExit;
    public static event System.Action OnReactionComplete;

    [Header("Reaction Parameters")]
    [SerializeField] private int requiredAl = 2;
    [SerializeField] private int requiredNaOH = 2;
    [SerializeField] private int requiredH2O = 6;
    [SerializeField] private int requiredBalloons = 3;

    [Header("UI References")]
    [SerializeField] private GameObject labPanel;
    [SerializeField] private SliderWithLabel alSlider;
    [SerializeField] private SliderWithLabel naohSlider;
    [SerializeField] private SliderWithLabel h2oSlider;
    [SerializeField] private GameObject reactButton;

    [Header("Events")]
    [SerializeField] private UnityEvent onReactionTriggered;   // Start animation, etc.

    private bool panelActive = false;

    private void Start()
    {
        labPanel.SetActive(false);
        reactButton.SetActive(false);

        // Hook up slider change listeners
        alSlider.onValueChanged.AddListener(OnSliderChanged);
        naohSlider.onValueChanged.AddListener(OnSliderChanged);
        h2oSlider.onValueChanged.AddListener(OnSliderChanged);

        // Listen for inventory changes (especially balloons)
        InventoryManager.Instance.OnItemCountChanged += OnInventoryCountChanged;
        UpdateSliderLimits();
    }

    private void OnDestroy()
    {
        if (InventoryManager.Instance != null)
            InventoryManager.Instance.OnItemCountChanged -= OnInventoryCountChanged;
    }

    /// <summary>
    /// Called when the player clicks the instrument (via VR or mouse).
    /// </summary>
    public void ToggleLabPanel()
    {
        panelActive = !panelActive;
        labPanel.SetActive(panelActive);

        if (panelActive)
        {
            UpdateSliderLimits();
            ResetSlidersToZero();
            OnLabEnter?.Invoke();
            EvaluateReactionCondition();
        }
        else
        {
            OnLabExit?.Invoke();
        }
    }

    private void UpdateSliderLimits()
    {
        var inv = InventoryManager.Instance;
        alSlider.maxValue = inv.GetCount(ItemType.Al);
        naohSlider.maxValue = inv.GetCount(ItemType.NaOH);
        h2oSlider.maxValue = inv.GetCount(ItemType.H2O);
    }

    private void ResetSlidersToZero()
    {
        alSlider.value = 0;
        naohSlider.value = 0;
        h2oSlider.value = 0;
    }

    private void OnSliderChanged(float _)
    {
        EvaluateReactionCondition();
    }

    private void OnInventoryCountChanged(ItemType type, int count)
    {
        // Only re-evaluate when inside the panel and balloon count changes
        if (panelActive && type == ItemType.Balloon)
            EvaluateReactionCondition();
    }

    private void EvaluateReactionCondition()
    {
        // Use integer comparison because sliders should be whole numbers.
        bool slidersReady = alSlider.value == requiredAl &&
                            naohSlider.value == requiredNaOH &&
                            h2oSlider.value == requiredH2O;

        bool hasEnoughBalloons = InventoryManager.Instance.GetCount(ItemType.Balloon) >= requiredBalloons;

        reactButton.SetActive(slidersReady && hasEnoughBalloons);
    }

    /// <summary>
    /// Called by the React button (UI or 3D).
    /// </summary>
    public void OnReactButtonPressed()
    {
        // Remove the used items
        InventoryManager.Instance.RemoveItem(ItemType.Al, requiredAl);
        InventoryManager.Instance.RemoveItem(ItemType.NaOH, requiredNaOH);
        InventoryManager.Instance.RemoveItem(ItemType.H2O, requiredH2O);

        // Update slider max values (they will have decreased)
        UpdateSliderLimits();
        ResetSlidersToZero();

        // Start the reaction animation / effects
        onReactionTriggered?.Invoke();

        // Balloons are awarded at the end of the animation via GiveBalloons()
    }

    /// <summary>
    /// Called by an Animation Event at the end of the reaction animation.
    /// </summary>
    public void GiveBalloons()
    {
        InventoryManager.Instance.AddItem(ItemType.Balloon, 3);
        OnReactionComplete?.Invoke();
    }
}