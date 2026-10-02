using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour
{
    [SerializeField] private float lifetime = 3f;

    private Rigidbody2D rb;
    private float damage;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
        GetComponent<Collider2D>().isTrigger = true;
    }

    public void Launch(Vector2 direction, float speed, float damageAmount)
    {
        damage = damageAmount;
        rb.linearVelocity = direction.normalized * speed;
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Ignore the Player
        if (other.GetComponentInParent<PlayerMovement>() != null) return;

        Health health = other.GetComponentInParent<Health>();
        if (health != null)
        {
            health.TakeDamage(damage);
            EnemyFollow enemy = other.GetComponentInParent<EnemyFollow>();
            if (enemy != null) enemy.Knockback(rb.linearVelocity, 4f, 0.15f);
            Destroy(gameObject);
        }
    }
}