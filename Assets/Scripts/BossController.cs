using UnityEngine;
using System.Collections;

public class BossController : MonoBehaviour
{
    [Header("Boss Settings")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float minX = -4f;
    [SerializeField] private float maxX = 4f;
    [SerializeField] private float minY = 1f;
    [SerializeField] private float maxY = 2f;
    [SerializeField] private float moveInterval = 2f;

    [Header("Attack Settings")]
    [SerializeField] private GameObject shotgunBulletPrefab;
    [SerializeField] private GameObject homingBulletPrefab;
    [SerializeField] private float shotgunCooldown = 2f;
    [SerializeField] private float homingCooldown = 3f;
    [SerializeField] private float homingBulletLifetime = 10f;
    [SerializeField] private float homingBulletSpeed = 3f;

    [Header("Shoot Points")]
    [SerializeField] private Transform shootPointLeft;
    [SerializeField] private Transform shootPointCenter;
    [SerializeField] private Transform shootPointRight;

    private Transform player;
    private float timeSinceLastShotgun;
    private float timeSinceLastHoming;
    private bool isMovingRight = true;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        StartCoroutine(MovePattern());
    }

    private void Update()
    {
        HandleAttacks();
        ClampPosition();
    }

    private void ClampPosition()
    {
        Vector3 currentPos = transform.position;
        currentPos.y = Mathf.Clamp(currentPos.y, minY, maxY);
        transform.position = currentPos;
    }

    private IEnumerator MovePattern()
    {
        while (true)
        {
            if (isMovingRight)
            {
                while (transform.position.x < maxX)
                {
                    transform.Translate(Vector2.right * moveSpeed * Time.deltaTime);
                    yield return null;
                }
            }
            else
            {
                while (transform.position.x > minX)
                {
                    transform.Translate(Vector2.left * moveSpeed * Time.deltaTime);
                    yield return null;
                }
            }

            isMovingRight = !isMovingRight;
            yield return new WaitForSeconds(moveInterval);
        }
    }

    private void HandleAttacks()
    {
        timeSinceLastShotgun += Time.deltaTime;
        timeSinceLastHoming += Time.deltaTime;

        if (timeSinceLastShotgun >= shotgunCooldown)
        {
            FireShotgun();
            timeSinceLastShotgun = 0f;
        }

        if (timeSinceLastHoming >= homingCooldown && player != null)
        {
            FireHoming();
            timeSinceLastHoming = 0f;
        }
    }

    private void FireShotgun()
    {
        if (shotgunBulletPrefab == null) return;

        // 左方向 (向左下方)
        if (shootPointLeft != null)
        {
            GameObject bulletLeft = Instantiate(shotgunBulletPrefab, shootPointLeft.position, Quaternion.Euler(0, 0, 210f));
            BulletController bulletControllerLeft = bulletLeft.GetComponent<BulletController>();
            if (bulletControllerLeft != null)
            {
                bulletControllerLeft.SetDirection(new Vector2(-1f, -1f).normalized);
                bulletControllerLeft.SetIsPlayerBullet(false);
            }
        }

        // 中心方向 (向下)
        if (shootPointCenter != null)
        {
            GameObject bulletCenter = Instantiate(shotgunBulletPrefab, shootPointCenter.position, Quaternion.Euler(0, 0, 180f));
            BulletController bulletControllerCenter = bulletCenter.GetComponent<BulletController>();
            if (bulletControllerCenter != null)
            {
                bulletControllerCenter.SetDirection(Vector2.down);
                bulletControllerCenter.SetIsPlayerBullet(false);
            }
        }

        // 右方向 (向右下方)
        if (shootPointRight != null)
        {
            GameObject bulletRight = Instantiate(shotgunBulletPrefab, shootPointRight.position, Quaternion.Euler(0, 0, 150f));
            BulletController bulletControllerRight = bulletRight.GetComponent<BulletController>();
            if (bulletControllerRight != null)
            {
                bulletControllerRight.SetDirection(new Vector2(1f, -1f).normalized);
                bulletControllerRight.SetIsPlayerBullet(false);
            }
        }
    }

    private void FireHoming()
    {
        if (homingBulletPrefab == null || player == null) return;

        GameObject bullet = Instantiate(homingBulletPrefab, transform.position, Quaternion.identity);
        HomingBulletController homingController = bullet.GetComponent<HomingBulletController>();
        if (homingController != null)
        {
            homingController.Initialize(player, homingBulletSpeed, homingBulletLifetime);
        }
        else
        {
            Destroy(bullet, homingBulletLifetime);
        }
    }

    public void OnBossDestroyed()
    {
        // Boss被摧毁时的逻辑
        Debug.Log("Boss destroyed!");
    }
}