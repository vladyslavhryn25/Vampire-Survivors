using UnityEngine;

public class ContactDamage : MonoBehaviour
{
    [SerializeField] private float damage = 10f;
    [SerializeField] private float damageInterval = 1f;

    private float nextDamageTime;

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (Time.time < nextDamageTime) return;

        if (!collision.collider.TryGetComponent<PlayerMovement>(out _)) return;

        if (collision.collider.TryGetComponent<Health>(out Health playerHealth))
        {
            playerHealth.TakeDamage(damage);
            nextDamageTime = Time.time + damageInterval;
        }
    }
}