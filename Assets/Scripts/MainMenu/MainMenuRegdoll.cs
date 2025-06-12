using UnityEngine;

public class test : MonoBehaviour
{
    public float forceMagnitude = 10f;      // 힘의 세기
    public float interval = 2f;             // 힘을 주는 간격 (초)
    private Rigidbody rb;
    private float timer = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        ApplyRandomForce();
    }

    void Update()
    {
        timer += Time.deltaTime;

        // interval(예: 2초)마다 실행
        if (timer >= interval)
        {
            ApplyRandomForce();
            timer = 0f;  // 타이머 초기화
        }
    }

    void ApplyRandomForce()
    {
        Vector3 randomDirection = Random.onUnitSphere;
        Vector3 force = randomDirection * forceMagnitude;
        rb.AddForce(force, ForceMode.Impulse);
    }
}
