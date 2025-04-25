using UnityEngine;
using System.Collections;

public class PickUpNote : MonoBehaviour
{
    [Header("Pickup Settings")]
    public Transform noteHoldPoint; // Пустой объект перед камерой (позиция для записки)
    public float pickupRange = 3f;
    public LayerMask pickupLayer;

    [Header("Note Settings")]
    public float rotationSpeed = 5f;
    public float moveDuration = 0.5f;
    public float maxRotation = 30f; // Максимальный угол поворота по всем осям

    [Header("Camera Settings")]
    public float cameraMaxRotation = 45f; // Максимальный угол поворота камеры
    public float noteRotationSensitivity = 2f; // Чувствительность вращения записки от движения камеры

    [Header("Audio Settings")]
    public AudioClip pickupSound;
    public AudioClip putAwaySound;
    private AudioSource audioSource;

    private GameObject heldNote;
    private Rigidbody noteRb;
    private bool isHolding = false;
    private float currentXRotation = 0f; // Текущий угол поворота по X
    private float currentYRotation = 0f; // Текущий угол поворота по Y
    private float currentZRotation = 0f; // Текущий угол поворота по Z
    private Quaternion initialRotation; // Начальная ротация записки
    
    private Vector3 originalCameraPosition;
    private Quaternion originalCameraRotation;
    private float currentCameraXRotation = 0f;
    private float currentCameraYRotation = 0f;
    private float lastCameraXRotation = 0f;
    private float lastCameraYRotation = 0f;

    private FPS_Controller fpsController;
    private FPS_Camera fpsCamera;
    private Camera mainCamera;

    void Start()
    {
        audioSource = GetComponent<AudioSource>() ?? gameObject.AddComponent<AudioSource>();

        fpsController = GetComponent<FPS_Controller>();
        fpsCamera = GetComponentInChildren<FPS_Camera>();
        mainCamera = Camera.main;

        if (fpsController == null)
            Debug.LogWarning("FPS_Controller not found!");
        if (fpsCamera == null)
            Debug.LogWarning("FPS_Camera not found!");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (!isHolding)
                TryPickupNote();
            else
                PutAwayNote();
        }

        if (isHolding && heldNote != null)
        {
            // Получаем ввод мыши по горизонтали и вертикали
            float mouseX = Input.GetAxis("Mouse X") * rotationSpeed;
            float mouseY = Input.GetAxis("Mouse Y") * rotationSpeed;
            
            // Сохраняем предыдущие значения углов камеры
            lastCameraXRotation = currentCameraXRotation;
            lastCameraYRotation = currentCameraYRotation;
            
            // Обновляем углы вращения камеры
            currentCameraYRotation += mouseX;
            currentCameraXRotation -= mouseY; // Инвертируем для естественного движения
            
            // Ограничиваем вращение камеры
            currentCameraXRotation = Mathf.Clamp(currentCameraXRotation, -cameraMaxRotation, cameraMaxRotation);
            currentCameraYRotation = Mathf.Clamp(currentCameraYRotation, -cameraMaxRotation, cameraMaxRotation);
            
            // Применяем вращение к камере
            Quaternion targetCameraRotation = originalCameraRotation * Quaternion.Euler(currentCameraXRotation, currentCameraYRotation, 0);
            mainCamera.transform.rotation = targetCameraRotation;
            
            // Вращение записки на основе движения камеры
            float deltaX = currentCameraXRotation - lastCameraXRotation;
            float deltaY = currentCameraYRotation - lastCameraYRotation;
            
            // Инвертируем направление вращения для более естественного ощущения
            currentYRotation += deltaX * noteRotationSensitivity; // Вращение по Y от движения мыши по X
            currentXRotation -= deltaY * noteRotationSensitivity; // Вращение по X от движения мыши по Y
            
            // Ограничиваем вращение записки по всем осям
            currentXRotation = Mathf.Clamp(currentXRotation, -maxRotation, maxRotation);
            currentYRotation = Mathf.Clamp(currentYRotation, -maxRotation, maxRotation);
            currentZRotation = Mathf.Clamp(currentZRotation, -maxRotation, maxRotation);
            
            // Применяем вращение по всем осям к записке
            Quaternion targetRotation = initialRotation * Quaternion.Euler(currentXRotation, currentYRotation, currentZRotation);
            heldNote.transform.rotation = targetRotation;
        }
    }

    void TryPickupNote()
    {
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, pickupRange, pickupLayer))
        {
            GameObject obj = hit.collider.gameObject;

            if (obj.CompareTag("Note"))
            {
                heldNote = obj;
                noteRb = heldNote.GetComponent<Rigidbody>();

                if (noteRb != null)
                {
                    noteRb.isKinematic = true;
                    heldNote.transform.SetParent(null); // Освобождаем перед анимацией

                    if (pickupSound != null)
                        audioSource.PlayOneShot(pickupSound);

                    isHolding = true;
                    currentXRotation = 0f;
                    currentYRotation = 0f;
                    currentZRotation = 0f;
                    
                    // Сохраняем исходное положение и поворот камеры
                    originalCameraPosition = mainCamera.transform.position;
                    originalCameraRotation = mainCamera.transform.rotation;
                    currentCameraXRotation = 0f;
                    currentCameraYRotation = 0f;
                    lastCameraXRotation = 0f;
                    lastCameraYRotation = 0f;

                    if (fpsController != null)
                        fpsController.enabled = false;
                    if (fpsCamera != null)
                        fpsCamera.enabled = false;

                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;

                    StartCoroutine(MoveNoteToHoldPoint());
                }
            }
        }
    }

    IEnumerator MoveNoteToHoldPoint()
    {
        float time = 0f;
        Vector3 startPos = heldNote.transform.position;
        Quaternion startRot = heldNote.transform.rotation;

        Vector3 endPos = noteHoldPoint.position;
        // Поворачиваем записку к лицу игрока с поворотом на 90 градусов по X
        Quaternion endRot = Quaternion.LookRotation(
            Camera.main.transform.position - noteHoldPoint.position) * Quaternion.Euler(90, 0, 0);

        while (time < 1f)
        {
            float t = time / moveDuration;
            heldNote.transform.position = Vector3.Lerp(startPos, endPos, t);
            heldNote.transform.rotation = Quaternion.Slerp(startRot, endRot, t);
            time += Time.deltaTime;
            yield return null;
        }

        heldNote.transform.position = endPos;
        heldNote.transform.rotation = endRot;
        initialRotation = endRot; 
        heldNote.transform.SetParent(noteHoldPoint);
    }

    void PutAwayNote()
    {
        if (heldNote != null && noteRb != null)
        {
            heldNote.transform.SetParent(null);
            noteRb.isKinematic = false;

            if (putAwaySound != null)
                audioSource.PlayOneShot(putAwaySound);

            // Восстанавливаем исходное положение и поворот камеры
            mainCamera.transform.position = originalCameraPosition;
            mainCamera.transform.rotation = originalCameraRotation;

            heldNote = null;
            noteRb = null;
            isHolding = false;

            if (fpsController != null)
                fpsController.enabled = true;
            if (fpsCamera != null)
                fpsCamera.enabled = true;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
