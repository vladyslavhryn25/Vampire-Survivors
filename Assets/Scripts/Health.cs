using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private bool destroyOnDeath = true; // on for Enemy, off for Player

    public float CurrentHealth { get; private set; }
    public float MaxHealth => maxHealth;
    public bool IsDead { get; private set; }

    public event Action Died;

    private void Awake()
    {
        CurrentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        if (IsDead) return;

        CurrentHealth = Mathf.Max(0f, CurrentHealth - amount);

        if (CurrentHealth <= 0f)
        {
            IsDead = true;
            Died?.Invoke();
            if (destroyOnDeath) Destroy(gameObject);
        }
    }

    // Test: right-click the Health component header in Play mode
    [ContextMenu("Test: Take 10 Damage")]
    private void TestDamage() => TakeDamage(10f);
}