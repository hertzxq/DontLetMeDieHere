using UnityEngine;

public class EnterInWater : MonoBehaviour
{
    [SerializeField] private Canvas gameOverCanvas;
    
    void Start()
    {
        Time.timeScale = 1;
        if (gameOverCanvas != null)
        {
            gameOverCanvas.gameObject.SetActive(false);
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (gameOverCanvas != null)
            {
                gameOverCanvas.gameObject.SetActive(true);
            }
            else
            {
                Debug.LogError("Game Over Canvas not assigned to EnterInWater script!");
            }
            
            // Destroy the player
            Destroy(other.gameObject);
        }
    }
}
