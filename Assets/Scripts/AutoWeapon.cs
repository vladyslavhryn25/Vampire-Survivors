using UnityEngine;

public class AutoWeapon : MonoBehaviour
{
    [SerializeField] private Projectile projectilePrefab;
    [SerializeField] private float damage = 10f;
    [SerializeField] private float projectileSpeed = 5f;
    [SerializeField] private float attackInterval = 0.8f;
    [SerializeField] private float range = 6f;
    [SerializeField] private float spawnOffset = 0.6f;
    [SerializeField] private bool onlyOnScreen = true;
    [SerializeField] private float screenMargin = 0.02f;

    private float nextAttackTime;
    private Camera cam;

    private void Awake()
    {
        cam = Camera.main;
    }

    private void Update()
    {
        if (projectilePrefab == null) return;
        if (Time.time < nextAttackTime) return;

        Transform target = FindNearestEnemy();
        if (target == null) return;

        nextAttackTime = Time.time + attackInterval;

        Vector2 direction = ((Vector2)target.position - (Vector2)transform.position).normalized;
        Vector2 spawnPosition = (Vector2)transform.position + direction * spawnOffset;

        Projectile projectile = Instantiate(projectilePrefab, spawnPosition, Quaternion.identity);
        projectile.Launch(direction, projectileSpeed, damage);
    }

    private Transform FindNearestEnemy()
    {
        EnemyFollow[] enemies = FindObjectsByType<EnemyFollow>(FindObjectsSortMode.None);
        Transform best = null;
        float bestDistance = range * range;

        foreach (EnemyFollow enemy in enemies)
        {
            if (onlyOnScreen && !IsOnScreen(enemy.transform.position)) continue;

            float distance = (enemy.transform.position - transform.position).sqrMagnitude;
            if (distance < bestDistance)
            {
                bestDistance = distance;
                best = enemy.transform;
            }
        }

        return best;
    }

    private bool IsOnScreen(Vector3 worldPosition)
    {
        if (cam == null) return true;

        Vector3 vp = cam.WorldToViewportPoint(worldPosition);
        return vp.z > 0f
            && vp.x > screenMargin && vp.x < 1f - screenMargin
            && vp.y > screenMargin && vp.y < 1f - screenMargin;
    }
}