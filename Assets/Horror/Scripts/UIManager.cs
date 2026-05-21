using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("UI Objects")]
    public TMP_Text storyText;

    public GameObject cheatGuidePanel;

    [Header("Puzzle Glow Objects")]
    public Renderer[] puzzleRenderers;

    private bool storyVisible = true;

    private bool cheatMode = false;

    // Purple glow color
    private Color cheatGlowColor =
        new Color(0.54f, 0.17f, 0.89f);

    // STORY BUTTON
    public void ToggleStory()
    {
        storyVisible = !storyVisible;

        storyText.gameObject.SetActive(storyVisible);
    }

    // CHEAT BUTTON
    public void ToggleCheatMode()
    {
        cheatMode = !cheatMode;

        // Show / Hide cheat guide panel
        cheatGuidePanel.SetActive(cheatMode);

        // Turn glow ON/OFF
        foreach (Renderer r in puzzleRenderers)
        {
            if (r != null)
            {
                if (cheatMode)
                {
                    // ENABLE GLOW
                    r.material.EnableKeyword("_EMISSION");

                    r.material.SetColor(
                        "_EmissionColor",
                        cheatGlowColor * 5f
                    );
                }
                else
                {
                    // DISABLE GLOW
                    r.material.SetColor(
                        "_EmissionColor",
                        Color.black
                    );
                }
            }
        }

        Debug.Log(
            cheatMode
            ? "Cheat Mode ON"
            : "Cheat Mode OFF"
        );
    }
}