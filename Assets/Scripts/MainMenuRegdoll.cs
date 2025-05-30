using UnityEngine;

public class test : MonoBehaviour
{
    public float forceMagnitude = 10f;      // ���� ����
    public float interval = 2f;             // ���� �ִ� ���� (��)
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

        // interval(��: 2��)���� ����
        if (timer >= interval)
        {
            ApplyRandomForce();
            timer = 0f;  // Ÿ�̸� �ʱ�ȭ
        }
    }

    void ApplyRandomForce()
    {
        Vector3 randomDirection = Random.onUnitSphere;
        Vector3 force = randomDirection * forceMagnitude;
        rb.AddForce(force, ForceMode.Impulse);
    }
}
