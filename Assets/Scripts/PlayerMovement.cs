using UnityEngine;
using UnityEngine.InputSystem; // Needed for the Movement to work

public class PlayerMovement : MonoBehaviour
{
    // These are to store the core components needed for this to work.
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 2.0f;
    
    private Rigidbody2D rb;
    private Vector2 moveInput;
    
    // This makes sure that the Awake is unable to be tampered with externally.
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // This gets called automatically when the player inputs something using Move.
    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per Fixed frame
    private void FixedUpdate()
    {
        // This multiplies the input via the movespeed which will propel where the player wants to go.
        Vector2 velocity = moveInput.normalized * moveSpeed;
        // This applies it to the 2D Rigidbody.
        rb.linearVelocity = velocity;
    }
}
