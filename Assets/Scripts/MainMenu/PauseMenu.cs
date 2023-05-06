using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;

    private InputAction pause;
    public ControlsInput playerControls;

    private GameObject go;

    public GameObject pauseMenuUi;

    void Awake()
    {
        playerControls = new ControlsInput();
        pause = playerControls.Controls.Pause;
        pause.Enable();
        go = GameObject.FindGameObjectWithTag("Player");

    }

    public void Update()
    {  
      if (pause.WasPressedThisFrame())
        {
            
            Debug.Log("Paused");
            if (GameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        pauseMenuUi.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
        go.GetComponent<AlpacaCharacter>().enabled = true;
    }

    public void Pause()
    {
        pauseMenuUi.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
        go.GetComponent<AlpacaCharacter>().enabled = false;
        playerControls.Enable();
    }

    public void LoadMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game.");
        Application.Quit();
    }
}
