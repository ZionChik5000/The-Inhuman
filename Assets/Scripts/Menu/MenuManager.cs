using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("Movement");
    }

    // Метод для загрузки сцены настроек
    public void OpenSettings()
    {
        SceneManager.LoadScene("SettingsScene");
    }

    // Метод для выхода из игры
    public void ExitGame()
    {
        Application.Quit();
    }
}
