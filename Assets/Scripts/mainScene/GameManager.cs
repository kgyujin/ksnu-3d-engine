using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private Vector3 _lastCheckpointPosition;
    private int _lastCheckpointId = -1; // 현재 체크포인트 순번

    public static bool isPaused = false;
    public static bool canPlayerMove = true;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Update()
    {
        if (!GameManager.canPlayerMove)
        {
            return;
        }
    }

    // 체크포인트 위치 갱신 조건부
    public void UpdateCheckpoint(Vector3 checkpointPos, int checkpointId)
    {
        if (checkpointId > _lastCheckpointId)
        {
            Debug.Log($"[GameManager] 체크포인트 갱신: ID {checkpointId}, 위치: {checkpointPos}");

            _lastCheckpointPosition = checkpointPos;
            _lastCheckpointId = checkpointId;

            if (checkpointId != 0)
            {
                PauseMenuUI pauseUI = FindObjectOfType<PauseMenuUI>();
                if (pauseUI != null)
                {
                    pauseUI.ShowCheckpointNotice();
                }

            }
        }
    }

    public void RespawnPlayer(GameObject player)
    {
        RaycastObjectMover mover = FindObjectOfType<RaycastObjectMover>();
        if (mover != null && mover.GetSelectedObject() != null)
        {
            mover.ForceReleaseSelectedObject();  // 잡은 오브젝트 강제 해제
        }

        Vector3 respawnPosition = _lastCheckpointPosition + Vector3.up;

        player.transform.position = respawnPosition;

        Rigidbody rb = player.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    public void ReloadSceneFromCheckpoint()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    // 씬 로드 후 플레이어 찾아서 리스폰 처리
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnSceneLoaded; // 중복 호출 방지

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            RespawnPlayer(player);
        }
    }
}
