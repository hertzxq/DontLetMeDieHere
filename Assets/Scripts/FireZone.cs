using UnityEngine;
using UnityEngine.UI;

public class FireZone : MonoBehaviour
{
    [Header("Настройки")]
    public ParticleSystem burnEffectPrefab;
    public Transform effectSpawnPoint;
    public float timerBonus = 10f;
    public Text timerText;

    private float currentTime = 60f; // Начальное значение таймера

    void Start()
    {
        UpdateTimerUI();
    }

    void Update()
    {
        if (currentTime > 0)
        {
            currentTime -= Time.deltaTime;
            UpdateTimerUI();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Pickup"))
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

        // Добавляем время
        currentTime += timerBonus;
        UpdateTimerUI();
    }

    void UpdateTimerUI()
    {
        if (timerText != null)
        {
            timerText.text = "Time: " + Mathf.Max(currentTime, 0).ToString("F1") + "s";
        }
    }
}
