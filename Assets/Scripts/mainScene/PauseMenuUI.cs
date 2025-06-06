using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PauseMenuUI : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private GameObject checkpointNoticeUI;
    [SerializeField] private float checkpointNoticeDuration = 2f;

    private bool isPaused = false;

    void Start()
    {
        Time.timeScale = 1f;
        isPaused = false;

        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(false);
        if (checkpointNoticeUI != null)
            checkpointNoticeUI.SetActive(false);

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

    public void OnLoadCheckpointButton()
    {
        Time.timeScale = 1f; // 시간 정지 해제
        GameManager.Instance.ReloadSceneFromCheckpoint();
    }

    public void OnMainMenuButton()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void OnRespawnFromCheckpointButton()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            GameManager.Instance.RespawnPlayer(player);
            ResumeGame();
        }
    }

    public void ShowCheckpointNotice()
    {
        if (checkpointNoticeUI != null)
        {
            checkpointNoticeUI.SetActive(true);
            StartCoroutine(HideCheckpointNoticeCoroutine());
        }
    }

    private IEnumerator HideCheckpointNoticeCoroutine()
    {
        yield return new WaitForSecondsRealtime(checkpointNoticeDuration);

        if (checkpointNoticeUI != null)
        {
            checkpointNoticeUI.SetActive(false);
        }
    }
}