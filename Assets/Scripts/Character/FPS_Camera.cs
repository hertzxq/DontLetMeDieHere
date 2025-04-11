using UnityEngine;

public class FPS_Camera : MonoBehaviour
{
    public Transform player; 
    public Transform headPosition; 

    [Header("Mouse Settings")]
    public float mouseSensitivity = 2f; 
    public float minY = -45f, maxY = 45f; 

    private float rotationX = 0f; 
    private float rotationY = 0f; 

    [Header("Head Bobbing")]
    public float bobFrequency = 2f;
    public float bobAmplitude = 0.05f;
    private float bobTimer;
    private float defaultCamY;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; 
        Cursor.visible = false;
        defaultCamY = transform.localPosition.y;
        
        rotationY = player.eulerAngles.y;
    }

    void Update()
    {
        HandleMouseLook();
        HandleHeadBobbing();
        UpdateCameraPosition();
    }

    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        
        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, minY, maxY);
        
        rotationY += mouseX;
        
        player.rotation = Quaternion.Euler(0f, rotationY, 0f); 
        transform.localRotation = Quaternion.Euler(rotationX, 0f, 0f);
    }

    void HandleHeadBobbing()
    {
        CharacterController controller = player.GetComponent<CharacterController>();

        if (controller.velocity.magnitude > 0.1f && controller.isGrounded)
        {
            bobTimer += Time.deltaTime * bobFrequency;
            float bobOffset = Mathf.Sin(bobTimer) * bobAmplitude;
            transform.localPosition = new Vector3(transform.localPosition.x, defaultCamY + bobOffset, transform.localPosition.z);
        }
        else
        {
            transform.localPosition = new Vector3(transform.localPosition.x, Mathf.Lerp(transform.localPosition.y, defaultCamY, Time.deltaTime * 5f), transform.localPosition.z);
        }
    }

    void UpdateCameraPosition()
    {
        transform.position = headPosition.position; 
    }
}
