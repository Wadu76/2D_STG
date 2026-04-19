using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float detectionRange = 8f;

    [Header("Behavior Settings")]
    [SerializeField] private float fireRate = 1.5f;
    [SerializeField] private GameObject enemyBulletPrefab;
    [SerializeField] private Transform firePoint;

    private Transform player;
    private Rigidbody2D rb;
    private float nextFireTime = 0f;
    private Vector2 moveDirection;
    private ObjectPool objectPool;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
        }
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
    }

    private void Start()
    {
        player = FindObjectOfType<PlayerController>()?.transform;
        GameObject poolObj = new GameObject("EnemyBulletPool");
        objectPool = poolObj.AddComponent<ObjectPool>();
        objectPool.Initialize(enemyBulletPrefab, 10);
        moveDirection = new Vector2(Random.Range(-1f, 1f), Random.Range(-0.3f, 0.3f)).normalized;
    }

    private float changeDirectionTimer = 0f;
    private float changeDirectionInterval = 2f;

    private void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        changeDirectionTimer += Time.deltaTime;
        if (changeDirectionTimer >= changeDirectionInterval)
        {
            moveDirection = new Vector2(Random.Range(-1f, 1f), Random.Range(-0.3f, 0.3f)).normalized;
            changeDirectionTimer = 0f;
        }

        Vector3 currentPos = transform.position;
        currentPos.x = Mathf.Clamp(currentPos.x, -7f, 7f);
        currentPos.y = Mathf.Clamp(currentPos.y, 1f, 4f);
        transform.position = currentPos;

        if (Time.time >= nextFireTime && distanceToPlayer < detectionRange)
        {
            TryShoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + moveDirection * moveSpeed * Time.fixedDeltaTime);
    }

    private void TryShoot()
    {
        if (enemyBulletPrefab == null || firePoint == null) return;

        try
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
                    Vector2 shootDirection = (player.position - firePoint.position).normalized;
                    bulletCtrl.Initialize(shootDirection, 5f, 10, false);
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error shooting: " + e.Message);
        }
    }

    public void SetMoveSpeed(float speed)
    {
        moveSpeed = speed;
    }

    private void OnDisable()
    {
    }

    private void ReturnToPool()
    {
        if (objectPool != null && gameObject.activeInHierarchy)
        {
            objectPool.ReturnObject(gameObject);
        }
    }
}
