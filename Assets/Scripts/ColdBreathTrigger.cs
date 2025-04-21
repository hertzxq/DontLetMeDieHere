using UnityEngine;
using System.Collections;

public class ColdBreathTrigger : MonoBehaviour
{
    [SerializeField] private GameObject breathEffect;
    [SerializeField] private float activationDelay = 2f;
    
    private Coroutine activationCoroutine;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (breathEffect != null)
                breathEffect.SetActive(false);
                
            if (activationCoroutine != null)
            {
                StopCoroutine(activationCoroutine);
                activationCoroutine = null;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (breathEffect != null)
                activationCoroutine = StartCoroutine(ActivateBreathEffectWithDelay());
        }
    }
    
    private IEnumerator ActivateBreathEffectWithDelay()
    {
        yield return new WaitForSeconds(activationDelay);
        
        if (breathEffect != null)
            breathEffect.SetActive(true);
    }
}
