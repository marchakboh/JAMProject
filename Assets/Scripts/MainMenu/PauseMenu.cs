using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject PlayerCharacter;
    [SerializeField] private GameObject pauseMenuUi;

    private ControlsInput.PauseActions actions;
    private int buttonIndex = 0;

    public void SetActions(ControlsInput.PauseActions pause_actions)
    {
        actions = pause_actions;

        actions.Up.performed += TrySelectNextButtonUp;
        actions.Down.performed += TrySelectNextButtonDown;
        actions.Select.performed += TrySelectButton;
        actions.Close.performed += ClosePause;
    }

    private void TrySelectNextButtonUp(InputAction.CallbackContext ctx)
    {
        if (buttonIndex == 0) return;

        for (int index = 0; index < pauseMenuUi.transform.childCount; index++)
        {
            GameObject buttonObj = pauseMenuUi.transform.GetChild(index).gameObject;
            if (buttonObj && index == buttonIndex - 1)
            {
                Button button = buttonObj.GetComponent<Button>();
                button.Select();
                buttonIndex--;
                break;
            }
        }
    }

    private void TrySelectNextButtonDown(InputAction.CallbackContext ctx)
    {
        if (buttonIndex == (pauseMenuUi.transform.childCount - 1)) return;

        for (int index = 0; index < pauseMenuUi.transform.childCount; index++)
        {
            GameObject buttonObj = pauseMenuUi.transform.GetChild(index).gameObject;
            if (buttonObj && index == buttonIndex + 1)
            {
                Button button = buttonObj.GetComponent<Button>();
                button.Select();
                buttonIndex++;
                break;
            }
        }
    }

    private void TrySelectButton(InputAction.CallbackContext ctx)
    {
        for (int index = 0; index < pauseMenuUi.transform.childCount; index++)
        {
            GameObject buttonObj = pauseMenuUi.transform.GetChild(index).gameObject;
            if (buttonObj && index == buttonIndex)
            {
                Button button = buttonObj.GetComponent<Button>();
                button.onClick.Invoke();
            }
        }
    }

    private void ClosePause(InputAction.CallbackContext ctx)
    {
        Resume();
    }

    public void Resume()
    {
        actions.Up.performed -= TrySelectNextButtonUp;
        actions.Down.performed -= TrySelectNextButtonDown;
        actions.Select.performed -= TrySelectButton;
        actions.Close.performed -= ClosePause;

        pauseMenuUi.SetActive(false);
        Time.timeScale = 1f;
        PlayerCharacter.GetComponent<AlpacaCharacter>().enabled = true;
    }

    public void Pause()
    {
        pauseMenuUi.SetActive(true);
        Time.timeScale = 0f;
        PlayerCharacter.GetComponent<AlpacaCharacter>().enabled = false;
    }

    public void LoadMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
