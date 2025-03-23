using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("TestScene");
    }

    // Метод для загрузки сцены настроек
    public void OpenSettings()
    {
        SceneManager.LoadScene("Settings", LoadSceneMode.Additive);
    }

    // Метод для выхода из игры
    public void ExitGame()
    {
        Application.Quit();
    }
}
