using UnityEngine;
using UnityEngine.SceneManagement;

public class DeadScreen : MonoBehaviour
{
    [SerializeField] private GameObject deathMenuUI;

    public void Die()
    {
        ToggleUIElements(false);
        deathMenuUI.SetActive(true);
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void ToggleUIElements(bool state)
    {
        foreach (Transform child in transform)
        {
            if (child.gameObject != deathMenuUI)
            {
                child.gameObject.SetActive(state);
            }
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ExitToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MenuScene");
    }
}