using UnityEngine;

public class PickupableItem : MonoBehaviour
{
    public GameObject pickupPromptPrefab;
    private GameObject currentPrompt;
    private bool isPromptVisible = false;
    public float displayHeight = 1f;

    private Transform cameraTransform;

    void Start()
    {
        if (pickupPromptPrefab != null)
        {
            currentPrompt = Instantiate(pickupPromptPrefab, transform);
            currentPrompt.transform.localScale = Vector3.one * 0.01f;
            currentPrompt.transform.localPosition = new Vector3(0f, displayHeight, 0f);
            currentPrompt.transform.localRotation = Quaternion.identity;

            currentPrompt.SetActive(false);

            // Получаем ссылку на MainCamera
            cameraTransform = Camera.main != null ? Camera.main.transform : null;

            if (cameraTransform == null)
            {
                Debug.LogWarning("Не найдена MainCamera для LookAt");
            }
        }
        else
        {
            Debug.LogWarning($"Префаб UI не назначен для {gameObject.name}!");
        }
    }

    void Update()
    {
        if (currentPrompt != null && isPromptVisible && cameraTransform != null)
        {
            Vector3 directionToCamera = cameraTransform.position - currentPrompt.transform.position;
            directionToCamera.y = 0f; // Не наклоняем вверх/вниз
            currentPrompt.transform.rotation = Quaternion.LookRotation(directionToCamera);
            currentPrompt.transform.Rotate(0f, 180f, 0f); // 🔄 Поворачиваем на 180°
            Debug.Log($"{gameObject.name} Scale: {transform.lossyScale}");


        }
    }

    public void ShowPrompt(bool show)
    {
        if (currentPrompt != null && show != isPromptVisible)
        {
            currentPrompt.SetActive(show);
            isPromptVisible = show;
            Debug.Log($"{gameObject.name}: Надпись {(show ? "показана" : "скрыта")}");
        }
    }

    void OnDestroy()
    {
        if (currentPrompt != null)
        {
            Destroy(currentPrompt);
        }
    }
}