using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("Movement");
    }

    // ����� ��� �������� ����� ��������
    public void OpenSettings()
    {
        SceneManager.LoadScene("SettingsScene");
    }

    // ����� ��� ������ �� ����
    public void ExitGame()
    {
        Application.Quit();
    }
}
