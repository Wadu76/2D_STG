using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float minX = -8f;
    [SerializeField] private float maxX = 8f;
    [SerializeField] private float minY = -4f;
    [SerializeField] private float maxY = 4f;

    private Vector2 moveDirection;
    private Rigidbody2D rb;
    private float speedMultiplier = 1f;
    private static PlayerController instance;

    public static PlayerController Instance
    {
        get { return instance; }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
        }
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
    }

    private void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        moveDirection = new Vector2(horizontal, vertical).normalized;
    }

    private void FixedUpdate()
    {
        Vector2 newPosition = rb.position + moveDirection * moveSpeed * speedMultiplier * Time.fixedDeltaTime;
        newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);
        newPosition.y = Mathf.Clamp(newPosition.y, minY, maxY);
        rb.MovePosition(newPosition);
    }

    public Vector2 GetPosition()
    {
        return rb.position;
    }

    public void AddSpeedBonus(float bonus)
    {
        speedMultiplier *= bonus;
        Debug.Log("Speed bonus applied! New multiplier: " + speedMultiplier);
    }

    public float GetSpeedMultiplier()
    {
        return speedMultiplier;
    }
}
