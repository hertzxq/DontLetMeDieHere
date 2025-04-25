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
    public bool isControllable = true;


    [Header("Footstep Settings")]
    public AudioSource footstepSource;
    public AudioClip[] footstepClips;
    public float stepInterval = 0.5f;

    private float stepTimer = 0f;


void Update()
{
    if (!isControllable) return;

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

        // Воспроизведение звука шагов
        if (isGrounded && move.magnitude > 0.1f)
        {
            stepTimer -= Time.deltaTime;
            if (stepTimer <= 0f)
            {
                PlayFootstep();
                stepTimer = stepInterval;
            }
        }
        else
        {
            stepTimer = 0f; // сброс таймера, если не двигаемся
        }
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

    void PlayFootstep()
    {
    if (footstepClips.Length == 0 || footstepSource == null)
        return;

    int index = Random.Range(0, footstepClips.Length);
    footstepSource.clip = footstepClips[index];
    footstepSource.Play();
    }

}