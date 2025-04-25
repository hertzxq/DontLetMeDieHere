using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject mainMenuCanvas;
    [SerializeField] private GameObject settingsCanvas;

    void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 1f;
        
        // Ensure main menu is active and settings is inactive at start
        if (mainMenuCanvas != null) mainMenuCanvas.SetActive(true);
        if (settingsCanvas != null) settingsCanvas.SetActive(false);
    }

    public void StartGame()
    {
        SceneManager.LoadSceneAsync("GameScene");
        Debug.Log("Загрузка сцены началась...");
    }

    public void OpenOptions()
    {
        if (mainMenuCanvas != null) mainMenuCanvas.SetActive(false);
        if (settingsCanvas != null) settingsCanvas.SetActive(true);
        Debug.Log("Открыть настройки...");
    }

    public void QuitGame()
    {
        Debug.Log("Выход из игры");
        Application.Quit();
    }
    
    public void ReturnToMainMenu()
    {
        SceneManager.LoadSceneAsync("Menu");
        Debug.Log("Возвращение в главное меню...");
    }
    
    public void CloseOptions()
    {
        if (settingsCanvas != null) settingsCanvas.SetActive(false);
        if (mainMenuCanvas != null) mainMenuCanvas.SetActive(true);
        Debug.Log("Закрыть настройки...");
    }
}
