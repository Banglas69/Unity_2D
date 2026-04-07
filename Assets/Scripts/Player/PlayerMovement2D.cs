using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement2D : MonoBehaviour
{
    public float moveSpeed = 5f;

    private Rigidbody2D rb;
    private Vector2 moveInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;
    }

    private void Update()
    {
        moveInput = Vector2.zero;

        if (Input.GetKey(KeyCode.LeftArrow)) moveInput.x = -1f;
        if (Input.GetKey(KeyCode.RightArrow)) moveInput.x = 1f;
        if (Input.GetKey(KeyCode.UpArrow)) moveInput.y = 1f;
        if (Input.GetKey(KeyCode.DownArrow)) moveInput.y = -1f;

        moveInput = moveInput.normalized;
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = moveInput * moveSpeed;
    }
}