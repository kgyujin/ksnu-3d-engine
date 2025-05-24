using UnityEngine;

public class ThrowerTriggerzoneChild : MonoBehaviour
{
    private ThrowerPlatformParents throwerPlatform;

    private void Start()
    {
        throwerPlatform = GetComponentInParent<ThrowerPlatformParents>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Rigidbody rb = other.attachedRigidbody;
        if (rb != null)
        {
            Debug.Log($"[Trigger Enter] {rb.name} 진입");

            // 발사판 앞쪽 5m 위쪽 1m 위치로 날려보내기
            Vector3 defaultTarget = throwerPlatform.transform.position
                                  + throwerPlatform.transform.forward * 5f
                                  + Vector3.up * 1.5f;

            throwerPlatform?.RegisterObject(rb);
        }
    }


    private void OnTriggerExit(Collider other)
    {
        Rigidbody rb = other.attachedRigidbody;
        if (rb != null)
        {
            Debug.Log($"[Trigger Exit] {rb.name} 이탈");
            throwerPlatform?.UnregisterObject(rb);
        }
    }
}
