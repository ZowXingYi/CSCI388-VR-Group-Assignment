using UnityEngine;
using TMPro;

public class FinalRoomManager : MonoBehaviour
{
    [Header("Trophy Interaction")]
    public GameObject finalTrophy;
    [Tooltip("Distance check fallback for desktop testing.")]
    public float interactionDistance = 3f;

    [Header("Celebration Effects")]
    public ParticleSystem confettiEffect;
    public ParticleSystem sparkleEffect;

    [Header("Fire Light Settings")]
    public Light trophySpotLight;
    [Tooltip("How fast the fire light dances/flickers.")]
    public float fireFlickerSpeed = 9f;
    [Tooltip("How drastically the fire intensity jumps up and down.")]
    public float fireFlickerStrength = 0.3f;

    [Header("UI / Team Display")]
    public GameObject victoryPanel;
    public TextMeshProUGUI victoryText;

    [Header("Audio")]
    public AudioSource backgroundMusicSource;
    public AudioSource victorySoundSource;
    [Range(0f, 1f)]
    public float ambientVictoryVolume = 0.2f;

    [Header("Player (Desktop Test Fallback)")]
    public Transform playerCamera;

    private bool hasCelebrated = false;
    private float targetBaseIntensity = 1.5f;

    void Start()
    {
        // Hide victory board at the start
        if (victoryPanel != null)
            victoryPanel.SetActive(false);

        // Stop effects at the start
        if (confettiEffect != null)
            confettiEffect.Stop();

        if (sparkleEffect != null)
            sparkleEffect.Stop();

        // Start ambient background music
        if (backgroundMusicSource != null)
        {
            backgroundMusicSource.loop = true;
            backgroundMusicSource.Play();
        }

        // Set initial fire light properties
        if (trophySpotLight != null)
        {
            targetBaseIntensity = 1.5f;
            trophySpotLight.color = new Color(1f, 0.55f, 0.15f); // Rich amber fire color
        }
    }

    void Update()
    {
        // Dynamic Fire Flicker Engine
        if (trophySpotLight != null)
        {
            float noise = Mathf.PerlinNoise(Time.time * fireFlickerSpeed, 0f);
            float flickerMod = targetBaseIntensity * fireFlickerStrength;
            trophySpotLight.intensity = targetBaseIntensity + (noise - 0.5f) * flickerMod;
        }

        // Skip distance checks if already triggered
        if (hasCelebrated)
            return;

        // Desktop testing check: Allows pressing 'E' if close enough
        if (finalTrophy != null && playerCamera != null)
        {
            float distance = Vector3.Distance(playerCamera.position, finalTrophy.transform.position);
            if (distance <= interactionDistance)
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    StartCelebration();
                }
            }
        }
    }

    public void StartCelebration()
    {
        if (hasCelebrated) return;
        hasCelebrated = true;

        // 1. Play celebration visual effects
        if (confettiEffect != null)
            confettiEffect.Play();

        if (sparkleEffect != null)
            sparkleEffect.Play();

        // 2. Audio Management (Ducks ambient music, plays fanfare)
        if (victorySoundSource != null)
            victorySoundSource.Play();

        if (backgroundMusicSource != null)
            backgroundMusicSource.volume = ambientVictoryVolume;

        // 3. Make the fire light flare up significantly (Roaring Bonfire)
        targetBaseIntensity = 7.0f;
        fireFlickerSpeed = 15f;

        // 4. Reveal victory board
        if (victoryPanel != null)
            victoryPanel.SetActive(true);

        // 5. Populate structured team text layout
        if (victoryText != null)
        {
            victoryText.text =
                "<b><size=140%><color=#FFD700>VICTORY TEAM</color></size></b>\n" +
                "<size=90%><color=#CCCCCC>Developing Group</color></size>\n\n" +
                "<size=115%><b>Zow Xing Yi</b> — 8336830\n" +
                "<b>Chow Wun Yee Winny</b> — 8336416\n" +
                "<b>Kai Ling</b> — 8496845\n" +
                "<b>Yan ZiXi</b> — 8336556</size>\n\n\n" +
                "<size=85%><color=#FFA500><i>Congratulations! You completed the challenge.</i></color></size>";
        }
    }
}