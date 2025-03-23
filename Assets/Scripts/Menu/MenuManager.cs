using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("TestScene");
    }

    // ����� ��� �������� ����� ��������
    public void OpenSettings()
    {
        SceneManager.LoadScene("Settings", LoadSceneMode.Additive);
    }

    // ����� ��� ������ �� ����
    public void ExitGame()
    {
        Application.Quit();
    }
}
