using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class FindingVRMenuController : MonoBehaviour
{
    [Header("Menu / Hint Panel")]
    [SerializeField] private GameObject menuPanel;           // Main menu panel (can contain hints)
    [SerializeField] private FindingHintSystem hintSystem;          // Reference to HintSystem

    [Header("Controller Input")]
    [SerializeField] private InputActionReference menuButton; // Usually the "Menu" button

    private bool isMenuOpen = false;

    private void OnEnable()
    {
        if (menuButton != null && menuButton.action != null)
        {
            menuButton.action.performed += OnMenuButtonPressed;
        }
    }

    private void OnDisable()
    {
        if (menuButton != null && menuButton.action != null)
        {
            menuButton.action.performed -= OnMenuButtonPressed;
        }
    }

    private void OnMenuButtonPressed(InputAction.CallbackContext context)
    {
        ToggleMenu();
    }

    public void ToggleMenu()
    {
        isMenuOpen = !isMenuOpen;

        if (menuPanel != null)
        {
            menuPanel.SetActive(isMenuOpen);
        }

        if (isMenuOpen && hintSystem != null)
        {
            // Automatically show the next relevant hint when opening menu
            hintSystem.ShowNextHint();
        }
        else if (hintSystem != null)
        {
            hintSystem.HideHint();
        }
    }

    // Public methods for UI buttons if needed
    public void OpenMenu()
    {
        isMenuOpen = true;
        if (menuPanel != null) menuPanel.SetActive(true);
        if (hintSystem != null) hintSystem.ShowNextHint();
    }

    public void CloseMenu()
    {
        isMenuOpen = false;
        if (menuPanel != null) menuPanel.SetActive(false);
        if (hintSystem != null) hintSystem.HideHint();
    }
}