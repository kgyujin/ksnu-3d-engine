using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    // 인스펙터에서 설정할 고유 순번
    [SerializeField] private int checkpointId = 0;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log($"[체크포인트] ID {checkpointId} 도달 - 위치: {transform.position}");

            GameManager.Instance.UpdateCheckpoint(transform.position, checkpointId);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        // Box Collider 기준으로 그릴 사이즈 결정
        BoxCollider collider = GetComponent<BoxCollider>();
        if (collider != null)
        {
            // Collider 크기와 위치 반영
            Vector3 size = Vector3.Scale(collider.size, transform.lossyScale);
            Vector3 center = transform.position + collider.center;

            Gizmos.DrawWireCube(center, size);
        }
    }
}
