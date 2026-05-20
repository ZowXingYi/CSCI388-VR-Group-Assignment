// Manages the lab area trigger and the reaction logic.
// It shows/hides a UI panel with sliders and handles the reaction.

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

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
    [SerializeField] private int requiredBalloons = 3;   // total moles of balloons needed

    [Header("UI References")]
    [SerializeField] private GameObject labPanel;
    [SerializeField] private Slider alSlider;
    [SerializeField] private Slider naohSlider;
    [SerializeField] private Slider h2oSlider;
    [SerializeField] private GameObject reactButton;

    private bool panelActive = false;

    private void Start()
    {
        labPanel.SetActive(false);
        reactButton.SetActive(false);

        // Hook up slider change listeners
        alSlider.onValueChanged.AddListener(OnSliderChanged);
        naohSlider.onValueChanged.AddListener(OnSliderChanged);
        h2oSlider.onValueChanged.AddListener(OnSliderChanged);

        // Listen for inventory changes (now passes itemCount + totalMoles)
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
        // Sliders work with total moles (the amount available for reaction)
        alSlider.maxValue = inv.GetMoleCount(ItemType.Al);
        naohSlider.maxValue = inv.GetMoleCount(ItemType.NaOH);
        h2oSlider.maxValue = inv.GetMoleCount(ItemType.H2O);
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

    // Updated signature: (ItemType type, int itemCount, int totalMoles)
    private void OnInventoryCountChanged(ItemType type, int itemCount, int totalMoles)
    {
        // Re-evaluate only when inside the panel and balloon count (in moles) changes
        if (panelActive && type == ItemType.Balloon)
            EvaluateReactionCondition();
    }

    private void EvaluateReactionCondition()
    {
        // Slider values are moles of chemicals to use
        bool slidersReady = alSlider.value == requiredAl &&
                            naohSlider.value == requiredNaOH &&
                            h2oSlider.value == requiredH2O;

        // Balloons are checked by total moles (as added by GiveBalloons)
        bool hasEnoughBalloons = InventoryManager.Instance.GetMoleCount(ItemType.Balloon) == requiredBalloons;

        reactButton.SetActive(slidersReady && hasEnoughBalloons);
    }

    /// <summary>
    /// Called by the React button (UI or 3D).
    /// </summary>
    public void OnReactButtonPressed()
    {
        // Remove the used moles of chemicals
        InventoryManager.Instance.RemoveItem(ItemType.Al, requiredAl);
        InventoryManager.Instance.RemoveItem(ItemType.NaOH, requiredNaOH);
        InventoryManager.Instance.RemoveItem(ItemType.H2O, requiredH2O);

        // Update slider max values (they will have decreased)
        UpdateSliderLimits();
        ResetSlidersToZero();
    }
}