using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;

    public GameObject pauseMenuUI;
    public GameObject mainPauselPanel;
    public GameObject pauseSettingsPanel;

    private void Update()
    {
        if(Input.GetKey(KeyCode.Escape) || Input.GetButtonDown("Pause"))
        {
            if(GameIsPaused)
            {
                Resume();
            }else
            {
                Pause();
            }
        }
    }

    public void returnToMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        mainPauselPanel.SetActive(false);
        pauseSettingsPanel.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        mainPauselPanel.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
    }
}
