using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int currentHealth;

    [Header("Effects")]
    [SerializeField] private GameObject hitEffect;
    [SerializeField] private GameObject destroyEffect;

    private bool isDead = false;
    private EnemySpawner spawner;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    private void Start()
    {
        spawner = FindObjectOfType<EnemySpawner>();
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

        if (destroyEffect != null)
        {
            Instantiate(destroyEffect, transform.position, Quaternion.identity);
        }

        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.AddEnemyKillScore();
        }

        if (spawner != null)
        {
            spawner.OnEnemyDestroyed();
        }

        ReturnToPool();
    }

    private void ReturnToPool()
    {
        if (gameObject.activeInHierarchy)
        {
            EnemySpawner spawner = FindObjectOfType<EnemySpawner>();
            if (spawner != null)
            {
                spawner.OnEnemyDestroyed();
            }
            
            try
            {
                ObjectPool enemyPool = FindObjectOfType<ObjectPool>();
                
                if (enemyPool != null)
                {
                    enemyPool.ReturnObject(gameObject);
                }
                else
                {
                    Destroy(gameObject);
                }
            }
            catch (System.Exception e)
            {
                Debug.LogWarning("Error returning enemy to pool: " + e.Message);
                Destroy(gameObject);
            }
        }
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    public float GetHealthPercentage()
    {
        return (float)currentHealth / maxHealth;
    }
}
