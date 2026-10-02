using UnityEngine;

[RequireComponent(typeof(Health))]
public class PlayerDeath : MonoBehaviour
{
    private Health health;

    private void Awake()
    {
        health = GetComponent<Health>();
    }

    private void OnEnable()  => health.Died += OnPlayerDied;
    private void OnDisable() => health.Died -= OnPlayerDied;

    private void OnPlayerDied()
    {
        GetComponent<PlayerMovement>().enabled = false;

        AutoWeapon weapon = GetComponent<AutoWeapon>();
        if (weapon != null) weapon.enabled = false;

        PlayerFacing facing = GetComponent<PlayerFacing>();
        if (facing != null) facing.enabled = false;

        GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
        Debug.Log("GAME OVER"); // replaced by a Game Over screen in step 6
    }
}