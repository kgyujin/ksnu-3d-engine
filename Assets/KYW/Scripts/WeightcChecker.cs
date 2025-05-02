using UnityEngine;
using System.Collections.Generic;

public class WeightChecker : MonoBehaviour
{
    public float pressMinMass = 20f;

    private Foothold foothold;
    private Door door;

    private HashSet<Rigidbody> trackedBodies = new HashSet<Rigidbody>();
    private bool isPressed = false;

    private void Awake()
    {
        foothold = GetComponent<Foothold>();
        door = GetComponentInChildren<Door>();

        Debug.Log("[WeightChecker] 초기화 완료");
    }

    private void OnCollisionEnter(Collision collision)
    {
        Rigidbody rb = collision.rigidbody;
        if (rb != null)
        {
            trackedBodies.Add(rb);
            Debug.Log($"[충돌 시작] 추가됨: {rb.name}, 질량 = {rb.mass}");
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        Rigidbody rb = collision.rigidbody;
        if (rb != null)
        {
            trackedBodies.Remove(rb);
            Debug.Log($"[충돌 종료] 제거됨: {rb.name}, 질량 = {rb.mass}");
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        // 지속적으로 충돌 상태를 감지하고 물체가 겹쳐 있는지 확인
        Rigidbody rb = collision.rigidbody;
        if (rb != null && !trackedBodies.Contains(rb))
        {
            trackedBodies.Add(rb);
            Debug.Log($"[충돌 지속] 추가됨: {rb.name}, 질량 = {rb.mass}");
        }
    }

    private void FixedUpdate()
    {
        float totalMass = 0f;

        // 현재 충돌 중인 모든 리지드바디의 질량을 합산
        foreach (var rb in trackedBodies)
        {
            if (rb != null)
                totalMass += rb.mass;
        }

        Debug.Log($"[무게 측정] 현재 총 질량: {totalMass} / 임계값: {pressMinMass}");

        // 질량이 임계값 이상이면 발판을 눌러주고 문을 엽니다
        if (totalMass >= pressMinMass && !isPressed)
        {
            foothold.Press();
            door.Open();
            isPressed = true;
            Debug.Log($"[상태 변화] 임계값 도달 → 발판 눌림 & 문 열림 (총 질량: {totalMass})");
        }
        // 임계값 미만이면 발판을 원위치하고 문을 닫습니다
        else if (totalMass < pressMinMass && isPressed)
        {
            foothold.Release();
            door.Close();
            isPressed = false;
            Debug.Log($"[상태 변화] 임계값 미달 → 발판 원위치 & 문 닫힘 (총 질량: {totalMass})");
        }
    }
}
