using UnityEngine;

public class BossHealth : MonoBehaviour
{
    [SerializeField] private float maxHealth = 500f; // 普通敌人的5倍
    [SerializeField] private float currentHealth;
    [SerializeField] private int scoreValue = 1000;
    [SerializeField] private int expValue = 500;

    private BossController bossController;

    private void Start()
    {
        currentHealth = maxHealth;
        bossController = GetComponent<BossController>();
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // 增加分数
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.AddScore(scoreValue);
        }
        
        // 增加经验
        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.AddExp(expValue);
        }

        // 调用Boss被摧毁的方法
        if (bossController != null)
        {
            bossController.OnBossDestroyed();
        }

        // 通知EnemySpawner Boss已被摧毁
        EnemySpawner spawner = FindObjectOfType<EnemySpawner>();
        if (spawner != null)
        {
            spawner.OnEnemyDestroyed();
        }

        // 销毁Boss
        Destroy(gameObject);
    }

    public float GetHealthPercentage()
    {
        return currentHealth / maxHealth;
    }

    public float GetCurrentHealth()
    {
        return currentHealth;
    }

    public float GetMaxHealth()
    {
        return maxHealth;
    }
}