using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private Vector3 _lastCheckpointPosition;
    private int _lastCheckpointId = -1; // 현재 체크포인트 순번

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
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
        }
    }

    public void RespawnPlayer(GameObject player)
    {
        Vector3 respawnPosition = _lastCheckpointPosition + Vector3.up;

        player.transform.position = respawnPosition;

        Rigidbody rb = player.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }
}
