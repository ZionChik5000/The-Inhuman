using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private GameObject deathMenuUI;
    private bool isPaused = false;

    private void Start()
    {
        InitializeMenu();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                Resume();
            }
            else
            {
                TogglePause();
            }
        }
    }

    private void InitializeMenu()
    {
        ToggleChildObjects(gameObject, true);
        pauseMenuUI.SetActive(false);
        deathMenuUI.SetActive(false);
        SetPauseState(false);
    }

    private void TogglePause()
    {
        isPaused = !isPaused;
        pauseMenuUI.SetActive(isPaused);
        ToggleChildObjects(gameObject, !isPaused);
        SetPauseState(isPaused);
    }

    public void Resume()
    {
        isPaused = false;
        ToggleChildObjects(gameObject, true);
        pauseMenuUI.SetActive(false);
        deathMenuUI.SetActive(false);
        SetPauseState(isPaused);
    }

    private void SetPauseState(bool state)
    {
        Time.timeScale = state ? 0f : 1f;
        Cursor.lockState = state ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = state;
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void ToggleChildObjects(GameObject parent, bool state)
    {
        foreach (Transform child in parent.transform)
        {
            if (child.gameObject != pauseMenuUI)
            {
                child.gameObject.SetActive(state);
            }
        }
    }

    public void OpenSettings()
    {
        SceneManager.LoadScene("Settings", LoadSceneMode.Additive);
    }

    public void ExitGame()
    {
        SceneManager.LoadScene("MenuScene");
    }
}
