using UnityEngine;

[RequireComponent(typeof(Health))]
public class EnemyReward : MonoBehaviour
{
    [SerializeField] private int points = 10;

    private Health health;

    private void Awake()
    {
        health = GetComponent<Health>();
    }

    private void OnEnable()
    {
        health.Died += OnDied;
    }

    private void OnDisable()
    {
        health.Died -= OnDied;
    }

    private void OnDied()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.AddKill(points);
    }
}
