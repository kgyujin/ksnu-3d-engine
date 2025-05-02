using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class Foothold : MonoBehaviour
{
    public float pressedY = -0.4f;
    public float originalY;
    public float moveSpeed = 3f;  // 발판 이동 속도 (부드럽게 조정)

    private Rigidbody rb;
    private Collider platformCollider;
    private float targetY;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true; // 물리 영향 받지 않지만 MovePosition으로 움직일 수 있음

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
        // Y축만 Lerp로 부드럽게 이동
        Vector3 currentPos = transform.position;
        float newY = Mathf.Lerp(currentPos.y, targetY, Time.deltaTime * moveSpeed);

        // X, Z는 그대로 두고, Y만 부드럽게 이동
        rb.MovePosition(new Vector3(currentPos.x, newY, currentPos.z));
    }

    // SetTrigger는 현재 불필요하지만 남겨둠
    public void SetTrigger(bool isTrigger)
    {
        if (platformCollider != null)
            platformCollider.isTrigger = isTrigger;
    }
}
