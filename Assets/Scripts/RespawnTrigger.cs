using UnityEngine;
using System.Collections.Generic;

public class RespawnTrigger : MonoBehaviour
{
    // Selectable 레이어 지정
    [SerializeField] private LayerMask selectableLayer;

    // 오브젝트별 초기 위치 저장
    private Dictionary<GameObject, Vector3> _initialPositions = new Dictionary<GameObject, Vector3>();
    private Dictionary<GameObject, Quaternion> _initialRotations = new Dictionary<GameObject, Quaternion>();

    private void Start()
    {
        // 씬에 있는 모든 Selectable 오브젝트 초기 위치 저장
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        foreach (GameObject obj in allObjects)
        {
            // 레이어 비교
            if (((1 << obj.layer) & selectableLayer) != 0)
            {
                _initialPositions[obj] = obj.transform.position;
                _initialRotations[obj] = obj.transform.rotation;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // ----------------------
        // 플레이어 리스폰
        // ----------------------
        if (other.CompareTag("Player"))
        {
            Debug.Log("[리스폰] 플레이어 낙사 감지. 리스폰 시도");

            GameManager.Instance.RespawnPlayer(other.gameObject);
        }

        // ----------------------
        // Selectable 레이어 오브젝트 리스폰
        // ----------------------
        else if (((1 << other.gameObject.layer) & selectableLayer) != 0)
        {
            GameObject obj = other.gameObject;
            if (_initialPositions.ContainsKey(obj))
            {
                obj.transform.position = _initialPositions[obj];
                obj.transform.rotation = _initialRotations[obj];

                Rigidbody rb = obj.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.linearVelocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                }
            }
        }
    }
}
