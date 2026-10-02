using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerFacing : MonoBehaviour
{
    [SerializeField] private bool spriteFacesRight = true; // untick if the knight looks the wrong way
    [SerializeField] private float searchInterval = 0.1f;  // how often to look for the nearest enemy

    private SpriteRenderer sprite;
    private Rigidbody2D rb;
    private Transform nearestEnemy;
    private float nextSearchTime;

    private void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (Time.time >= nextSearchTime)
        {
            nextSearchTime = Time.time + searchInterval;
            nearestEnemy = FindNearestEnemy();
        }

        float directionX = 0f;

        if (nearestEnemy != null)
            directionX = nearestEnemy.position.x - transform.position.x;
        else
            directionX = rb.linearVelocity.x; // fall back to movement direction

        if (Mathf.Abs(directionX) > 0.01f)
        {
            bool lookRight = directionX > 0f;
            sprite.flipX = spriteFacesRight ? !lookRight : lookRight;
        }
    }

    private Transform FindNearestEnemy()
    {
        EnemyFollow[] enemies = FindObjectsByType<EnemyFollow>(FindObjectsSortMode.None);
        Transform best = null;
        float bestDistance = float.MaxValue;

        foreach (EnemyFollow enemy in enemies)
        {
            float distance = (enemy.transform.position - transform.position).sqrMagnitude;
            if (distance < bestDistance)
            {
                bestDistance = distance;
                best = enemy.transform;
            }
        }

        return best;
    }
}