using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    // This is for the input system using the new Unity Input system I learned from Workshop 2 PRAPI
    InputAction movement;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        movement = InputSystem.actions.FindAction("Move");
    }

    // Update is called once per frame
    void Update()
    {
        // This grabs the current input
        Vector2 moveValue = movement.ReadValue<Vector2>();

        // This sets it to the value. However it doesn't change the position based on location, only based on input.
        this.gameObject.transform.position = moveValue;
        
    }
}
