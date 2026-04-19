using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PauseMenuManager : MonoBehaviour
{
    [Header("Pause Panel")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private bool isPaused = false;

    [Header("Player Stats UI")]
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI speedText;
    [SerializeField] private TextMeshProUGUI damageText;
    [SerializeField] private TextMeshProUGUI fireRateText;
    [SerializeField] private TextMeshProUGUI iceChanceText;
    [SerializeField] private TextMeshProUGUI shotgunText;

    private void Start()
    {
        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            Time.timeScale = 0f;
            if (pausePanel != null)
            {
                pausePanel.SetActive(true);
                UpdatePlayerStats();
            }
        }
        else
        {
            Time.timeScale = 1f;
            if (pausePanel != null)
            {
                pausePanel.SetActive(false);
            }
        }
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }
    }

    private void UpdatePlayerStats()
    {
        if (PlayerHealth.Instance != null)
        {
            int currentHealth = PlayerHealth.Instance.GetCurrentHealth();
            healthText.text = "Health: " + currentHealth;
        }
        else
        {
            healthText.text = "Health: --";
        }

        if (PlayerController.Instance != null)
        {
            float speedBonus = PlayerController.Instance.GetSpeedMultiplier();
            speedText.text = "Speed: x" + speedBonus.ToString("F2");
        }
        else
        {
            speedText.text = "Speed: --";
        }

        if (PlayerShooter.Instance != null)
        {
            float damageMultiplier = PlayerShooter.Instance.GetDamageMultiplier();
            damageText.text = "Damage: x" + damageMultiplier.ToString("F2");

            float fireRateMultiplier = PlayerShooter.Instance.GetFireRateMultiplier();
            fireRateText.text = "Fire Rate: x" + fireRateMultiplier.ToString("F2");

            float iceChance = PlayerShooter.Instance.GetIceBulletChance();
            iceChanceText.text = "Ice Chance: " + (iceChance * 100).ToString("F0") + "%";

            bool hasShotgun = PlayerShooter.Instance.HasShotgun();
            shotgunText.text = hasShotgun ? "Shotgun: ON" : "Shotgun: OFF";
        }
        else
        {
            damageText.text = "Damage: --";
            fireRateText.text = "Fire Rate: --";
            iceChanceText.text = "Ice Chance: --";
            shotgunText.text = "Shotgun: --";
        }
    }

    public bool IsPaused()
    {
        return isPaused;
    }
}