using UnityEngine;

public class ItemHolder : MonoBehaviour
{
    [Header("Hand Settings")]
    public Transform handTransform; // Позиция руки
    private GameObject currentItem; // Текущий предмет в руке

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (handTransform == null)
        {
            // Пытаемся найти существующий объект руки
            handTransform = transform.Find("Hand");
            
            // Если не нашли, создаем новый
            if (handTransform == null)
            {
                Debug.Log("Creating new Hand object");
                GameObject handObject = new GameObject("Hand");
                handObject.transform.parent = transform;
                // Устанавливаем позицию руки перед игроком
                handObject.transform.localPosition = new Vector3(0.5f, 0.5f, 1f);
                handTransform = handObject.transform;
            }
        }
        Debug.Log($"Hand position: {handTransform.position}");
    }

    public void HoldItem(GameObject item)
    {
        if (handTransform == null)
        {
            Debug.LogError("Hand transform is not set!");
            return;
        }

        Debug.Log($"Before pickup - Item position: {item.transform.position}");
        
        // Сначала отвязываем от текущего родителя
        item.transform.SetParent(null);
        
        // Отключаем физику
        Rigidbody rb = item.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }
        
        // Отключаем коллайдер
        Collider itemCollider = item.GetComponent<Collider>();
        if (itemCollider != null)
        {
            itemCollider.enabled = false;
        }
        
        // Принудительно устанавливаем позицию и привязываем к руке
        item.transform.position = handTransform.position;
        item.transform.rotation = handTransform.rotation;
        item.transform.SetParent(handTransform);
        
        // Точная подстройка локальной позиции
        item.transform.localPosition = Vector3.zero;
        item.transform.localRotation = Quaternion.identity;
        
        currentItem = item;
        
        Debug.Log($"After pickup - Item position: {item.transform.position}");
        Debug.Log($"Hand position: {handTransform.position}");
    }

    // Дополнительные методы, которые могут пригодиться
    public void DropItem()
    {
        if (currentItem != null)
        {
            Debug.Log("Dropping item: " + currentItem.name);
            
            Rigidbody rb = currentItem.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;
            }
            
            Collider itemCollider = currentItem.GetComponent<Collider>();
            if (itemCollider != null)
            {
                itemCollider.enabled = true;
            }
            
            currentItem.transform.SetParent(null);
            currentItem = null;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
