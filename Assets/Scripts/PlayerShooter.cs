using UnityEngine;

public class PlayerShooter : MonoBehaviour
{
    [Header("Shooting Settings")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireRate = 0.2f;
    [SerializeField] private float bulletSpeed = 10f;
    [SerializeField] private int baseDamage = 10;

    private float nextFireTime = 0f;
    private ObjectPool objectPool;
    private float damageMultiplier = 1f;
    private float fireRateMultiplier = 1f;
    private bool hasShotgun = false;
    private float iceBulletChance = 0f;
    private static PlayerShooter instance;

    public static PlayerShooter Instance
    {
        get { return instance; }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        GameObject poolObj = new GameObject("PlayerBulletPool");
        objectPool = poolObj.AddComponent<ObjectPool>();
        objectPool.Initialize(bulletPrefab, 20);
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Space) || Input.GetMouseButton(0))
        {
            TryShoot();
        }
    }

    private void TryShoot()
    {
        if (Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + (fireRate * (1f / fireRateMultiplier));
        }
    }

    private void Shoot()
    {
        if (bulletPrefab == null)
        {
            Debug.LogError("Bullet prefab not assigned!");
            return;
        }

        if (firePoint == null)
        {
            Debug.LogError("Fire point not assigned!");
            return;
        }

        try
        {
            if (hasShotgun)
            {
                // 散弹模式：发射三发子弹
                ShootShotgun();
            }
            else
            {
                // 普通模式：发射一发子弹
                ShootSingleBullet();
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error in Shoot: " + e.Message);
        }
    }

    private void ShootSingleBullet()
    {
        GameObject bullet = objectPool.GetObject();
        if (bullet != null)
        {
            bullet.transform.position = firePoint.position;
            bullet.transform.rotation = Quaternion.identity;
            bullet.SetActive(true);

            Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
            if (bulletRb == null)
            {
                bulletRb = bullet.AddComponent<Rigidbody2D>();
                bulletRb.gravityScale = 0f;
            }
            bulletRb.velocity = Vector2.zero;

            BulletController bulletCtrl = bullet.GetComponent<BulletController>();
            if (bulletCtrl != null)
            {
                int finalDamage = Mathf.RoundToInt(baseDamage * damageMultiplier);
                bulletCtrl.Initialize(Vector2.up, bulletSpeed, finalDamage, true);

                // 检查是否触发寒冰子弹效果
                if (iceBulletChance > 0 && Random.value <= iceBulletChance)
                {
                    bulletCtrl.EnableIceEffect();
                }
            }
            else
            {
                Debug.LogError("Bullet prefab missing BulletController script!");
            }
        }
    }

    private void ShootShotgun()
    {
        // 发射三发子弹：左、中、右
        Vector2[] directions = new Vector2[]
        {
            new Vector2(-0.3f, 1f).normalized, // 左
            Vector2.up, // 中
            new Vector2(0.3f, 1f).normalized // 右
        };

        foreach (Vector2 direction in directions)
        {
            GameObject bullet = objectPool.GetObject();
            if (bullet != null)
            {
                bullet.transform.position = firePoint.position;
                bullet.transform.rotation = Quaternion.identity;
                bullet.SetActive(true);

                Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
                if (bulletRb == null)
                {
                    bulletRb = bullet.AddComponent<Rigidbody2D>();
                    bulletRb.gravityScale = 0f;
                }
                bulletRb.velocity = Vector2.zero;

                BulletController bulletCtrl = bullet.GetComponent<BulletController>();
                if (bulletCtrl != null)
                {
                    int finalDamage = Mathf.RoundToInt(baseDamage * damageMultiplier * 0.7f); // 散弹伤害降低30%
                    bulletCtrl.Initialize(direction, bulletSpeed, finalDamage, true);

                    // 检查是否触发寒冰子弹效果
                    if (iceBulletChance > 0 && Random.value <= iceBulletChance)
                    {
                        bulletCtrl.EnableIceEffect();
                    }
                }
                else
                {
                    Debug.LogError("Bullet prefab missing BulletController script!");
                }
            }
        }
    }

    public void AddDamageBonus(float bonus)
    {
        damageMultiplier *= bonus;
        Debug.Log("Damage bonus applied! New multiplier: " + damageMultiplier);
    }

    public void AddFireRateBonus(float bonus)
    {
        fireRateMultiplier *= bonus;
        Debug.Log("Fire rate bonus applied! New multiplier: " + fireRateMultiplier);
    }

    public void EnableShotgun()
    {
        hasShotgun = true;
        Debug.Log("Shotgun enabled!");
    }

    public void AddIceBulletChance(float chance)
    {
        iceBulletChance += chance;
        Debug.Log("Ice bullet chance increased! New chance: " + (iceBulletChance * 100) + "%");
    }

    public float GetDamageMultiplier()
    {
        return damageMultiplier;
    }

    public float GetFireRateMultiplier()
    {
        return fireRateMultiplier;
    }

    public float GetIceBulletChance()
    {
        return iceBulletChance;
    }

    public bool HasShotgun()
    {
        return hasShotgun;
    }
}
