using UnityEngine;

public class camera_test2 : MonoBehaviour
{
    public Transform target;       // ���� ��� (ĳ����)
    public Vector3 offset = new Vector3(0f, 5f, -7f); // ī�޶� ��ġ ������
    public float smoothSpeed = 10f;

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        transform.LookAt(target);
    }
}
