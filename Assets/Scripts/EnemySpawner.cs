using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Camera cam;

    [Header("Spawn Timing")]
    [SerializeField] private float startInterval = 1.5f;
    [SerializeField] private float minInterval = 0.3f;
    [SerializeField] private float rampDuration = 300f;
    [SerializeField] private int enemiesPerSpawn = 1;

    [Header("Spawn Area")]
    [SerializeField] private float minOffscreenMargin = 1f;
    [SerializeField] private float maxOffscreenMargin = 3f;

    [Header("Limits")]
    [SerializeField] private int maxAlive = 150;

    [Header("Straggler Recycling")]
    [SerializeField] private bool recycleStragglers = true;
    [SerializeField] private float recycleExtraDistance = 6f;
    [SerializeField] private float recycleCheckInterval = 1f;

    private float timer;
    private float elapsed;
    private float recycleTimer;
    private bool spawning = true;
    private Health playerHealth;
    private readonly List<GameObject> alive = new List<GameObject>();

    public void StopSpawning()
    {
        spawning = false;
    }

    public void StartSpawning()
    {
        spawning = true;
    }

    private void Awake()
    {
        if (cam == null) cam = Camera.main;
        if (player != null) playerHealth = player.GetComponent<Health>();
    }

    private void OnEnable()
    {
        if (playerHealth != null) playerHealth.Died += StopSpawning;
    }

    private void OnDisable()
    {
        if (playerHealth != null) playerHealth.Died -= StopSpawning;
    }

    private void Update()
    {
        if (!spawning) return;
        if (playerHealth != null && playerHealth.IsDead) return;
        if (player == null || !player.gameObject.activeInHierarchy) return;
        if (cam == null) return;

        elapsed += Time.deltaTime;
        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            timer = CurrentInterval();
            for (int i = 0; i < enemiesPerSpawn; i++)
                TrySpawn();
        }

        if (recycleStragglers)
        {
            recycleTimer -= Time.deltaTime;
            if (recycleTimer <= 0f)
            {
                recycleTimer = recycleCheckInterval;
                RecycleStragglers();
            }
        }
    }

    private float CurrentInterval()
    {
        float t = Mathf.Clamp01(elapsed / rampDuration);
        return Mathf.Lerp(startInterval, minInterval, t);
    }

    private void TrySpawn()
    {
        alive.RemoveAll(e => e == null);
        if (alive.Count >= maxAlive) return;

        GameObject enemy = Instantiate(enemyPrefab, GetSpawnPosition(), Quaternion.identity);
        alive.Add(enemy);
    }

    private void RecycleStragglers()
    {
        float halfH = cam.orthographicSize;
        float halfW = halfH * cam.aspect;
        float limit = Mathf.Sqrt(halfW * halfW + halfH * halfH) + maxOffscreenMargin + recycleExtraDistance;
        float limitSqr = limit * limit;

        for (int i = alive.Count - 1; i >= 0; i--)
        {
            GameObject enemy = alive[i];
            if (enemy == null)
            {
                alive.RemoveAt(i);
                continue;
            }

            float distSqr = ((Vector2)enemy.transform.position - (Vector2)player.position).sqrMagnitude;
            if (distSqr > limitSqr)
                enemy.transform.position = GetSpawnPosition();
        }
    }

    private Vector2 GetSpawnPosition()
    {
        Vector2 center = player.position;
        float halfH = cam.orthographicSize;
        float halfW = halfH * cam.aspect;

        float innerW = halfW + minOffscreenMargin;
        float innerH = halfH + minOffscreenMargin;
        float outerW = halfW + maxOffscreenMargin;
        float outerH = halfH + maxOffscreenMargin;

        for (int i = 0; i < 20; i++)
        {
            float x = Random.Range(-outerW, outerW);
            float y = Random.Range(-outerH, outerH);

            if (Mathf.Abs(x) > innerW || Mathf.Abs(y) > innerH)
                return center + new Vector2(x, y);
        }

        return center + new Vector2(innerW, 0f);
    }

    private void OnDrawGizmosSelected()
    {
        Camera c = cam != null ? cam : Camera.main;
        if (player == null || c == null) return;

        float halfH = c.orthographicSize;
        float halfW = halfH * c.aspect;

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(player.position, new Vector3((halfW + minOffscreenMargin) * 2f, (halfH + minOffscreenMargin) * 2f, 0f));
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(player.position, new Vector3((halfW + maxOffscreenMargin) * 2f, (halfH + maxOffscreenMargin) * 2f, 0f));
    }
}