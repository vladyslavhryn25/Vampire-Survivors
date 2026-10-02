using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;

    private Rigidbody2D rb;
    private Vector2 input;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
    }

    private void Update()
    {
        input.x = Input.GetAxisRaw("Horizontal"); // WASD + arrows
        input.y = Input.GetAxisRaw("Vertical");
        input = input.normalized; // no faster on diagonals
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = input * moveSpeed;
    }
}