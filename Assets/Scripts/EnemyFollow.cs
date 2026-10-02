using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyFollow : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2.5f;

    private Rigidbody2D rb;
    private Transform target;
    private float knockbackEndTime;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
    }

    private void Start()
    {
        PlayerMovement player = FindFirstObjectByType<PlayerMovement>();
        if (player != null) target = player.transform;
    }

    public void Knockback(Vector2 direction, float force, float duration)
    {
        knockbackEndTime = Time.time + duration;
        rb.linearVelocity = direction.normalized * force;
    }

    private void FixedUpdate()
    {
        if (Time.time < knockbackEndTime) return; // let knockback velocity play out

        if (target == null)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        Vector2 direction = ((Vector2)target.position - rb.position).normalized;
        rb.linearVelocity = direction * moveSpeed;
    }
}