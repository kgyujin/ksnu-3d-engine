using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class Foothold : MonoBehaviour
{
    public float pressedY = -0.4f;
    public float originalY;
    public float moveSpeed = 3f;  // ���� �̵� �ӵ� (�ε巴�� ����)

    private Rigidbody rb;
    private Collider platformCollider;
    private float targetY;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true; // ���� ���� ���� ������ MovePosition���� ������ �� ����

        platformCollider = GetComponent<Collider>();
        platformCollider.isTrigger = false;

        originalY = transform.position.y;
        targetY = originalY;
    }

    public void Press()
    {
        targetY = pressedY;
    }

    public void Release()
    {
        targetY = originalY;
    }

    void Update()
    {
        // Y�ุ Lerp�� �ε巴�� �̵�
        Vector3 currentPos = transform.position;
        float newY = Mathf.Lerp(currentPos.y, targetY, Time.deltaTime * moveSpeed);

        // X, Z�� �״�� �ΰ�, Y�� �ε巴�� �̵�
        rb.MovePosition(new Vector3(currentPos.x, newY, currentPos.z));
    }

    // SetTrigger�� ���� ���ʿ������� ���ܵ�
    public void SetTrigger(bool isTrigger)
    {
        if (platformCollider != null)
            platformCollider.isTrigger = isTrigger;
    }
}
