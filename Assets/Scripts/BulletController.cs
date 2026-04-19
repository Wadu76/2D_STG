using UnityEngine;
using System.Collections;

public class BulletController : MonoBehaviour
{
    [Header("Bullet Settings")]
    [SerializeField] private float speed = 10f;
    [SerializeField] private int damage = 10;
    [SerializeField] private float lifeTime = 3f;
    [SerializeField] private bool isPlayerBullet = true;
    [SerializeField] private float freezeDuration = 1f;

    private Vector2 moveDirection;
    private Rigidbody2D rb;
    private ObjectPool objectPool;
    private float startTime;
    private bool hasIceEffect = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
        }
        rb.gravityScale = 0f;
        startTime = Time.time;
    }

    public void Initialize(Vector2 direction, float bulletSpeed, int bulletDamage, bool playerBullet)
    {
        moveDirection = direction.normalized;
        speed = bulletSpeed;
        damage = bulletDamage;
        isPlayerBullet = playerBullet;

        int layerIndex = playerBullet ? LayerMask.NameToLayer("PlayerBullet") : LayerMask.NameToLayer("EnemyBullet");
        if (layerIndex != -1)
        {
            gameObject.layer = layerIndex;
        }
        else
        {
            Debug.LogWarning("Layer not found, using default layer");
        }
        objectPool = FindObjectOfType<ObjectPool>();
        ResetStartTime();
    }

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + moveDirection * speed * Time.fixedDeltaTime);
        CheckBounds();
        CheckLifetime();
    }

    private void CheckLifetime()
    {
        if (Time.time - startTime >= lifeTime)
        {
            ReturnToPool();
        }
    }

    private void CheckBounds()
    {
        Vector2 screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));

        if (isPlayerBullet)
        {
            if (transform.position.y > screenBounds.y + 1f)
            {
                ReturnToPool();
            }
        }
        else
        {
            if (transform.position.y < -screenBounds.y - 1f)
            {
                ReturnToPool();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isPlayerBullet)
        {
            if (other.gameObject.CompareTag("Enemy"))
            {
                Debug.Log("Player bullet hit enemy!");
                EnemyHealth enemyHealth = other.gameObject.GetComponent<EnemyHealth>();
                if (enemyHealth != null)
                {
                    Debug.Log("EnemyHealth found, dealing damage: " + damage);
                    enemyHealth.TakeDamage(damage);
                }
                else
                {
                    Debug.LogWarning("Enemy missing EnemyHealth script!");
                }

                // 应用寒冰效果
                if (hasIceEffect)
                {
                    ApplyIceEffect(other.gameObject);
                }

                ReturnToPool();
            }
            else if (other.gameObject.CompareTag("Boss"))
            {
                Debug.Log("Player bullet hit boss!");
                BossHealth bossHealth = other.gameObject.GetComponent<BossHealth>();
                if (bossHealth != null)
                {
                    Debug.Log("BossHealth found, dealing damage: " + damage);
                    bossHealth.TakeDamage(damage);
                }
                else
                {
                    Debug.LogWarning("Boss missing BossHealth script!");
                }

                // 应用寒冰效果到Boss
                if (hasIceEffect)
                {
                    ApplyIceEffect(other.gameObject);
                }

                ReturnToPool();
            }
        }
        else
        {
            if (other.gameObject.CompareTag("Player"))
            {
                Debug.Log("Enemy bullet hit player!");
                PlayerHealth playerHealth = other.gameObject.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(damage);
                }
                ReturnToPool();
            }
        }
    }

    private void ReturnToPool()
    {
        if (objectPool != null)
        {
            objectPool.ReturnObject(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public int GetDamage()
    {
        return damage;
    }

    public bool IsPlayerBullet()
    {
        return isPlayerBullet;
    }

    public void SetDirection(Vector2 direction)
    {
        moveDirection = direction.normalized;
    }

    public void SetDamage(int newDamage)
    {
        damage = newDamage;
    }

    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }

    public void SetIsPlayerBullet(bool value)
    {
        isPlayerBullet = value;
    }

    public void SetLifeTime(float newLifeTime)
    {
        lifeTime = newLifeTime;
    }

    public void ResetStartTime()
    {
        startTime = Time.time;
    }

    public void EnableIceEffect()
    {
        hasIceEffect = true;
        // 可以在这里添加视觉效果，比如改变子弹颜色
        Debug.Log("Ice effect enabled on bullet!");
    }

    private void ApplyIceEffect(GameObject target)
    {
        if (target == null) return;

        EnemyController enemyController = target.GetComponent<EnemyController>();
        if (enemyController != null)
        {
            StartCoroutine(FreezeEnemy(enemyController));
        }

        BossController bossController = target.GetComponent<BossController>();
        if (bossController != null)
        {
            StartCoroutine(FreezeBoss(bossController));
        }
    }

    private IEnumerator FreezeEnemy(EnemyController enemy)
    {
        if (enemy == null) yield break;

        // 保存原始速度
        Rigidbody2D rb = enemy.gameObject.GetComponent<Rigidbody2D>();
        Vector2 originalVelocity = Vector2.zero;
        if (rb != null)
        {
            originalVelocity = rb.velocity;
            rb.velocity = Vector2.zero;
        }

        // 保存原始颜色并应用冰冻效果
        SpriteRenderer spriteRenderer = enemy.gameObject.GetComponent<SpriteRenderer>();
        Color originalColor = Color.white;
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
            spriteRenderer.color = new Color(0.5f, 0.8f, 1f, 0.8f); // 冰蓝色
        }

        // 禁用敌人AI
        enemy.enabled = false;

        // 等待冰冻持续时间
        yield return new WaitForSeconds(freezeDuration);

        // 恢复颜色
        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }

        // 恢复敌人AI
        enemy.enabled = true;

        // 恢复速度
        if (rb != null)
        {
            rb.velocity = originalVelocity;
        }
    }

    private IEnumerator FreezeBoss(BossController boss)
    {
        if (boss == null) yield break;

        // 保存原始速度
        Rigidbody2D rb = boss.gameObject.GetComponent<Rigidbody2D>();
        Vector2 originalVelocity = Vector2.zero;
        if (rb != null)
        {
            originalVelocity = rb.velocity;
            rb.velocity = Vector2.zero;
        }

        // 保存原始颜色并应用冰冻效果
        SpriteRenderer spriteRenderer = boss.gameObject.GetComponent<SpriteRenderer>();
        Color originalColor = Color.white;
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
            spriteRenderer.color = new Color(0.5f, 0.8f, 1f, 0.8f); // 冰蓝色
        }

        // 禁用Boss AI
        boss.enabled = false;

        // 等待冰冻持续时间
        yield return new WaitForSeconds(freezeDuration);

        // 恢复颜色
        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }

        // 恢复Boss AI
        boss.enabled = true;

        // 恢复速度
        if (rb != null)
        {
            rb.velocity = originalVelocity;
        }
    }
}
