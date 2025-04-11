using UnityEngine;

public class FPS_Controller : MonoBehaviour
{
    public CharacterController controller;
    public Transform cameraTransform; 

    [Header("Movement Settings")]
    public float speed = 6f;
    public float gravity = -9.81f;
    public float jumpHeight = 1.5f;
    private Vector3 velocity;
    private bool isGrounded;

    void Update()
    {
        HandleMovement();
        HandleJump();
        ApplyGravity();
    }

    void HandleMovement()
    {
        isGrounded = controller.isGrounded;
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;
        
        forward.y = 0;
        right.y = 0;
        
        forward.Normalize();
        right.Normalize();

        Vector3 move = (right * moveX + forward * moveZ) * speed;
        controller.Move(move * Time.deltaTime);
    }

    void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

    void ApplyGravity()
    {
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}