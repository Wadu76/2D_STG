using UnityEngine;

public class HomingBulletController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float rotationSpeed = 180f;
    [SerializeField] private int damage = 10;

    private Transform target;
    private float lifetime;
    private float startTime;

    public void Initialize(Transform targetTransform, float speed, float lifeTime)
    {
        target = targetTransform;
        moveSpeed = speed;
        lifetime = lifeTime;
        startTime = Time.time;
    }

    private void Update()
    {
        if (Time.time - startTime >= lifetime)
        {
            Destroy(gameObject);
            return;
        }

        if (target != null)
        {
            // 计算朝向目标的方向
            Vector2 direction = (target.position - transform.position).normalized;
            
            // 旋转子弹朝向目标
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
            Quaternion targetRotation = Quaternion.Euler(0, 0, angle);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            
            // 向前移动（这里的Vector2.up是相对于子弹自身的前方向）
            transform.Translate(Vector2.up * moveSpeed * Time.deltaTime);
        }
        else
        {
            // 如果目标丢失，继续直线飞行
            transform.Translate(Vector2.up * moveSpeed * Time.deltaTime);
        }
        
        CheckBounds();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }
            Destroy(gameObject);
        }
    }

    private void CheckBounds()
    {
        Vector2 screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
        
        if (transform.position.y < -screenBounds.y - 1f ||
            transform.position.x < -screenBounds.x - 1f ||
            transform.position.x > screenBounds.x + 1f)
        {
            Destroy(gameObject);
        }
    }
}