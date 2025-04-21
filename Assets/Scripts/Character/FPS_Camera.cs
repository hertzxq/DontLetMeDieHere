using UnityEngine;

public class FPS_Camera : MonoBehaviour
{
    public Transform player; 
    public Transform headPosition; 

    [Header("Mouse Settings")]
    public float mouseSensitivity = 2f; 
    public float minY = -60f, maxY = 60f; // Уменьшенный диапазон для стандартного FPS-обзора

    private float rotationX = 0f; 
    private float rotationY = 0f; 

    [Header("Head Bobbing")]
    public float bobFrequency = 2f;
    public float bobAmplitude = 0.05f;
    private float bobTimer;
    private float defaultCamY;

    [Header("UI")]
    public Canvas gameOverCanvas; // Ссылка на Canvas с UI окончания игры
    public Canvas pauseCanvas; // Ссылка на Canvas с UI паузы

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; 
        Cursor.visible = false;
        defaultCamY = transform.localPosition.y;
        
        rotationY = player.eulerAngles.y;
    }

    void Update()
    {
        if ((gameOverCanvas != null && gameOverCanvas.gameObject.activeInHierarchy) || 
            (pauseCanvas != null && pauseCanvas.gameObject.activeInHierarchy))
        {
            // Если активно UI окончания игры или паузы, не обрабатываем поворот камеры
            return;
        }

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