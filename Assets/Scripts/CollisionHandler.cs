using UnityEngine;

public class CollisionHandler : MonoBehaviour
{
    [Header("Player Reference")]
    [SerializeField] private PlayerController playerController;

    private void Awake()
    {
        if (playerController == null)
        {
            playerController = GetComponentInParent<PlayerController>();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlayerBullet"))
        {
            HandlePlayerBulletHit(other.gameObject);
        }
        else if (other.CompareTag("Enemy"))
        {
            HandleEnemyCollision(other.gameObject);
        }
    }

    private void HandlePlayerBulletHit(GameObject bullet)
    {
        BulletController bulletCtrl = bullet.GetComponent<BulletController>();
        if (bulletCtrl != null && bulletCtrl.IsPlayerBullet())
        {
            int damage = bulletCtrl.GetDamage();
            EnemyHealth enemyHealth = GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);
            }
            ReturnBulletToPool(bullet);
        }
    }

    private void HandleEnemyCollision(GameObject enemy)
    {
        if (enemy.CompareTag("Enemy"))
        {
            EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(50);
            }
        }
    }

    private void ReturnBulletToPool(GameObject bullet)
    {
        ObjectPool pool = FindObjectOfType<ObjectPool>();
        if (pool != null)
        {
            pool.ReturnObject(bullet);
        }
        else
        {
            Destroy(bullet);
        }
    }
}
