using System.Collections.Generic;
using UnityEngine;

public class ThrowerPlatformParents : MonoBehaviour
{
    [Tooltip("던질 거리 (로컬 forward 기준)")]
    public float throwDistance = 5f;

    [Tooltip("발사에 줄 힘의 크기")]
    public float throwForce = 10f;

    private List<Rigidbody> objectsOnPlatform = new List<Rigidbody>();

    public void RegisterObject(Rigidbody rb)
    {
        if (rb != null && !objectsOnPlatform.Contains(rb))
            objectsOnPlatform.Add(rb);
    }

    public void UnregisterObject(Rigidbody rb)
    {
        if (rb != null)
            objectsOnPlatform.Remove(rb);
    }

    public void LaunchObjects()
    {
        foreach (Rigidbody rb in objectsOnPlatform)
        {
            if (rb == null) continue;

            // 발사 방향: 발사판의 forward 방향
            Vector3 direction = transform.forward.normalized;

            // 원하는 거리와 힘에 비례한 속도 벡터
            Vector3 velocity = direction * throwForce;

            // 기존 속도 초기화 후 속도 직접 설정
            rb.linearVelocity = Vector3.zero;
            rb.linearVelocity = velocity;
        }

        objectsOnPlatform.Clear();
    }
}
