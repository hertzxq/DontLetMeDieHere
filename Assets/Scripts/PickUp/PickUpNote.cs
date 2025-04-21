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
    public float maxRotationY = 30f; // Максимальный угол поворота по оси Y
    public float maxRotationX = 30f; // Максимальный угол поворота по оси X

    [Header("Audio Settings")]
    public AudioClip pickupSound;
    public AudioClip putAwaySound;
    private AudioSource audioSource;

    private GameObject heldNote;
    private Rigidbody noteRb;
    private bool isHolding = false;
    private float currentYRotation = 0f; // Текущий угол поворота по Y
    private float currentXRotation = 0f; // Текущий угол поворота по X

    private FPS_Controller fpsController;
    private FPS_Camera fpsCamera;

    void Start()
    {
        audioSource = GetComponent<AudioSource>() ?? gameObject.AddComponent<AudioSource>();

        fpsController = GetComponent<FPS_Controller>();
        fpsCamera = GetComponentInChildren<FPS_Camera>();

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
            
            // Ограничиваем вращение по осям X и Y
            currentYRotation += mouseX;
            currentXRotation -= mouseY; // Инвертируем для естественного движения
            
            currentYRotation = Mathf.Clamp(currentYRotation, -maxRotationY, maxRotationY);
            currentXRotation = Mathf.Clamp(currentXRotation, -maxRotationX, maxRotationX);
            
            // Применяем вращение по осям X и Y, сохраняя исходное значение Z
            Quaternion targetRotation = Quaternion.Euler(currentXRotation, currentYRotation, heldNote.transform.eulerAngles.z);
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
                    currentYRotation = 0f; // Сбрасываем угол поворота при взятии записки
                    currentXRotation = 0f; // Сбрасываем угол поворота по X

                    if (fpsController != null)
                        fpsController.enabled = false;
                    if (fpsCamera != null)
                        fpsCamera.enabled = false;

                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;

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
        // Поворачиваем записку к лицу игрока
        Quaternion endRot = Quaternion.LookRotation(
            Camera.main.transform.position - noteHoldPoint.position);

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
