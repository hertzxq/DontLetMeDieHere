using UnityEngine;
using UnityEngine.UI;

public class TakeItem : MonoBehaviour
{
    [Header("Item Settings")]
    public float pickupRange = 3f;
    public bool isPickable = true;
    public AudioClip pickupSound;
    
    [Header("UI References")]
    public Text pickupPromptText;
    
    private bool playerInRange = false;
    private Transform player;
    private AudioSource audioSource;
    private ItemHolder itemHolder;

    void Start()
    {
        // Находим игрока
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
            itemHolder = playerObject.GetComponent<ItemHolder>();
            Debug.Log($"Player found: {playerObject.name}, ItemHolder component: {(itemHolder != null ? "Yes" : "No")}");
        }
        else
        {
            Debug.LogError("Player not found! Make sure player has 'Player' tag");
        }

        // Настраиваем аудио если есть
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null && pickupSound != null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Скрываем текст подсказки при старте
        if (pickupPromptText != null)
        {
            pickupPromptText.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (!isPickable || player == null) 
        {
            return;
        }
        
        // Проверяем расстояние до игрока
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        playerInRange = distanceToPlayer <= pickupRange;
        
        // Управляем видимостью подсказки
        if (pickupPromptText != null)
        {
            pickupPromptText.gameObject.SetActive(playerInRange);
            if (playerInRange)
            {
                pickupPromptText.text = "Press E to pick up";
            }
        }
        
        // Проверяем нажатие клавиши E
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log($"Attempting to pick up {gameObject.name}");
            PickupItem();
        }
    }
    
    void PickupItem()
    {
        if (itemHolder != null)
        {
            Vector3 originalPos = transform.position;
            Debug.Log($"Original position before pickup: {originalPos}");
            
            itemHolder.HoldItem(this.gameObject);
            
            // Проверяем, изменилась ли позиция
            if (transform.position == originalPos)
            {
                Debug.LogWarning("Position didn't change after pickup!");
            }
            
            // Проигрываем звук подбора
            if (audioSource != null && pickupSound != null)
            {
                AudioSource.PlayClipAtPoint(pickupSound, originalPos);
            }
            
            // Скрываем подсказку
            if (pickupPromptText != null)
            {
                pickupPromptText.gameObject.SetActive(false);
            }
        }
        else
        {
            Debug.LogError("ItemHolder component not found on player!");
        }
    }

    // Отрисовка радиуса подбора в редакторе
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, pickupRange);
    }
}
