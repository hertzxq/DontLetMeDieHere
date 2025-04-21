using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 1f;
    }

    public void StartGame()
    {
        SceneManager.LoadSceneAsync("GameScene");
        Debug.Log("Загрузка сцены началась...");
    }

    public void OpenOptions()
    {
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
}
