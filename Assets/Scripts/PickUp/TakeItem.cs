using UnityEngine;

public class TakeItem : MonoBehaviour
{
    public float pickupRange = 3f;
    public Transform holdPosition;
    private GameObject heldObject;
    private bool isHolding = false;
    public float throwForce = 10f;
    public LayerMask pickupLayer;

    private Collider playerCollider;

    void Start()
    {
        playerCollider = GetComponent<Collider>();
        if (playerCollider == null)
        {
            Debug.LogError("У игрока нет коллайдера!");
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (!isHolding)
            {
                TryPickup();
            }
            else
            {
                DropItem();
            }
        }

        if (isHolding && heldObject != null)
        {
            heldObject.transform.position = Vector3.Lerp(heldObject.transform.position, holdPosition.position, Time.deltaTime * 20f);
            if (Input.GetMouseButtonDown(1))
            {
                ThrowItem();
            }
        }

        CheckForPickupPrompt();
    }

    void TryPickup()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, pickupRange, pickupLayer);
        float closestDistance = pickupRange + 1f;
        GameObject closestObject = null;

        foreach (Collider hit in hitColliders)
        {
            if (hit.CompareTag("Pickupable"))
            {
                float distance = Vector3.Distance(transform.position, hit.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestObject = hit.gameObject;
                }
            }
        }

        if (closestObject != null)
        {
            heldObject = closestObject;
            Rigidbody rb = heldObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true;
            }

            Collider itemCollider = heldObject.GetComponent<Collider>();
            if (playerCollider != null && itemCollider != null)
            {
                Physics.IgnoreCollision(playerCollider, itemCollider, true);
            }

            isHolding = true;
            PickupableItem pickupable = heldObject.GetComponent<PickupableItem>();
            if (pickupable != null)
            {
                pickupable.ShowPrompt(false);
            }
        }
    }

    void DropItem()
    {
        if (heldObject != null)
        {
            Rigidbody rb = heldObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;
            }

            Collider itemCollider = heldObject.GetComponent<Collider>();
            if (playerCollider != null && itemCollider != null)
            {
                Physics.IgnoreCollision(playerCollider, itemCollider, false);
            }

            heldObject = null;
            isHolding = false;
        }
    }

    void ThrowItem()
    {
        if (heldObject != null)
        {
            Rigidbody rb = heldObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;
                rb.AddForce(transform.forward * throwForce, ForceMode.Impulse);
            }

            Collider itemCollider = heldObject.GetComponent<Collider>();
            if (playerCollider != null && itemCollider != null)
            {
                Physics.IgnoreCollision(playerCollider, itemCollider, false);
            }

            heldObject = null;
            isHolding = false;
        }
    }

    void CheckForPickupPrompt()
    {
        if (isHolding) return;

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, pickupRange, pickupLayer);

        foreach (Collider hit in hitColliders)
        {
            PickupableItem pickupable = hit.GetComponent<PickupableItem>();
            if (pickupable != null)
            {
                float distance = Vector3.Distance(transform.position, hit.transform.position);
                bool shouldShow = (distance <= pickupRange);
                pickupable.ShowPrompt(shouldShow && distance == GetClosestDistance(hitColliders));
            }
        }
    }

    float GetClosestDistance(Collider[] colliders)
    {
        float closestDistance = pickupRange + 1f;
        foreach (Collider hit in colliders)
        {
            if (hit.CompareTag("Pickupable"))
            {
                float distance = Vector3.Distance(transform.position, hit.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                }
            }
        }
        return closestDistance;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, pickupRange);
    }
}