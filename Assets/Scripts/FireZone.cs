using UnityEngine;
using UnityEngine.UI;

public class FireZone : MonoBehaviour
{
    [Header("Настройки")]
    public ParticleSystem burnEffectPrefab;
    public Transform effectSpawnPoint;
    public float timerBonus = 15f;
    public Text timerText;
    public AudioClip burnSound; 
    
    [Header("Game Over")]
    public Canvas gameOverCanvas; 
    public AudioClip gameOverSound;

    private float currentTime = 60f; 
    private AudioSource audioSource;
    private bool isGameOver = false;

    void Start()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;
        UpdateTimerUI();
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        // Убедимся, что Canvas с окончанием игры выключен в начале
        if (gameOverCanvas != null)
        {
            gameOverCanvas.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (!isGameOver && currentTime > 0)
        {
            currentTime -= Time.deltaTime;
            UpdateTimerUI();
            
            // Проверяем, закончилось ли время
            if (currentTime <= 0)
            {   
                GameOver();
    }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!isGameOver && other.CompareTag("Pickup"))
        {
            BurnObject(other.gameObject);
        }
    }

    void BurnObject(GameObject obj)
    {
        // Воспроизводим эффект
        if (burnEffectPrefab != null && effectSpawnPoint != null)
        {
            ParticleSystem effect = Instantiate(burnEffectPrefab, effectSpawnPoint.position, Quaternion.identity);
            effect.Play();
            Destroy(effect.gameObject, 3f);
        }

        // Удаляем предмет
        Destroy(obj);
        
        // Воспроизводим звук сжигания
        if (burnSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(burnSound);
        }

        // Добавляем время
        currentTime += timerBonus;
        UpdateTimerUI();
    }

    void UpdateTimerUI()
    {
        if (timerText != null)
        {
            timerText.text = "Время: " + Mathf.Max(currentTime, 0).ToString("F1") + "с";
        }
    }
    
void GameOver()
{
    isGameOver = true;
    currentTime = 0;
    UpdateTimerUI();

    if (gameOverCanvas != null)
    {
        gameOverCanvas.gameObject.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    // Отключаем управление:
    GameObject player = GameObject.FindWithTag("Player");
    if (player != null)
    {
        FPS_Controller controller = player.GetComponent<FPS_Controller>();
        if (controller != null)
            controller.isControllable = false;
    }

    Time.timeScale = 0f;
    AudioListener.pause = true;


    Debug.Log("Игра окончена! Время истекло.");
}

}
