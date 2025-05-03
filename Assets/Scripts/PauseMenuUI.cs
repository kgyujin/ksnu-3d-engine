using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuUI : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuUI;

    private bool isPaused = false;

    void Start()
    {
        Time.timeScale = 1f;
        isPaused = false;

        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isPaused)
                PauseGame();
            else
                ResumeGame();
        }
    }

    private void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;

        pauseMenuUI.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;

        pauseMenuUI.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void OnResumeButton()
    {
        ResumeGame();
    }

    public void OnMainMenuButton()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}
