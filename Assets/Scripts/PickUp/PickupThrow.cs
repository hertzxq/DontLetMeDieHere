using UnityEngine;

public class PickupThrow : MonoBehaviour
{
    [Header("Pickup Settings")]
    public Transform handHoldPoint;           // Точка, куда будет прикрепляться предмет
    public float pickupRange = 3f;            // Дистанция до предмета
    public LayerMask pickupLayer;             // Только предметы на этом слое можно поднять

    [Header("Throw Settings")]
    public float throwForce = 500f;

    private GameObject heldObject;
    private Rigidbody heldRb;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (heldObject == null)
            {
                TryPickup();
            }
            else
            {
                Throw();
            }
        }
    }

    void TryPickup()
    {
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, pickupRange, pickupLayer))
        {
            GameObject obj = hit.collider.gameObject;
            Debug.Log("Попали в объект: " + hit.collider.name);

            // Проверка на тег, если нужно
            if (obj.CompareTag("Pickup"))
            {
                heldObject = obj;
                heldRb = heldObject.GetComponent<Rigidbody>();

                if (heldRb != null)
                {
                    heldRb.isKinematic = true;
                    heldObject.transform.SetParent(handHoldPoint);
                    heldObject.transform.localPosition = Vector3.zero;
                    heldObject.transform.localRotation = Quaternion.identity;
                }
            }
        }

        Debug.DrawRay(transform.position, transform.forward * pickupRange, Color.red, 1f);
        Debug.Log("Пробуем подобрать...");

    }

    void Throw()
    {
        if (heldObject != null && heldRb != null)
        {
            heldObject.transform.SetParent(null);
            heldRb.isKinematic = false;

            heldRb.AddForce(transform.forward * throwForce);

            heldObject = null;
            heldRb = null;
        }
    }
}
