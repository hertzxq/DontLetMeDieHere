using UnityEngine;

public class PickupThrow : MonoBehaviour
{
    [Header("Pickup Settings")]
    public Transform handHoldPoint;          
    public float pickupRange = 3f;        
    public LayerMask pickupLayer;          

    [Header("Throw Settings")]
    public float throwForce = 500f;

    [Header("Audio Settings")]
    public AudioClip pickupSound;
    public AudioClip throwSound;
    private AudioSource audioSource;

    private GameObject heldObject;
    private Rigidbody heldRb;

    void Start()
    {
        // Get or add AudioSource component
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

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
                    
                    // Play pickup sound
                    if (pickupSound != null && audioSource != null)
                    {
                        audioSource.PlayOneShot(pickupSound);
                    }
                }
            }
        }

    }

    void Throw()
    {
        if (heldObject != null && heldRb != null)
        {
            heldObject.transform.SetParent(null);
            heldRb.isKinematic = false;

            heldRb.AddForce(transform.forward * throwForce);
            
            // Play throw sound
            if (throwSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(throwSound);
            }

            heldObject = null;
            heldRb = null;
        }
    }
}
