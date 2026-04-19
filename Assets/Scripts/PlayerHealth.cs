using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int currentHealth;

    [Header("Effects")]
    [SerializeField] private GameObject hitEffect;
    [SerializeField] private GameObject gameOverPanel;

    private bool isDead = false;
    private float healthMultiplier = 1f;
    private static PlayerHealth instance;

    public static PlayerHealth Instance
    {
        get { return instance; }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        
        currentHealth = Mathf.RoundToInt(maxHealth * healthMultiplier);
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;

        if (hitEffect != null)
        {
            Instantiate(hitEffect, transform.position, Quaternion.identity);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowGameOverPanel();
        }

        Time.timeScale = 0f;
        Debug.Log("Game Over!");
    }

    public void Heal(int amount)
    {
        if (isDead) return;
        int newMaxHealth = Mathf.RoundToInt(maxHealth * healthMultiplier);
        currentHealth = Mathf.Min(currentHealth + amount, newMaxHealth);
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    public float GetHealthPercentage()
    {
        int newMaxHealth = Mathf.RoundToInt(maxHealth * healthMultiplier);
        return (float)currentHealth / newMaxHealth;
    }

    public bool IsDead()
    {
        return isDead;
    }

    public void AddHealthBonus(float bonus)
    {
        healthMultiplier *= bonus;
        int newMaxHealth = Mathf.RoundToInt(maxHealth * healthMultiplier);
        currentHealth = newMaxHealth;
        Debug.Log("Health bonus applied! New max health: " + newMaxHealth);
    }
}
