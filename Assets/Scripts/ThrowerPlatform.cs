using UnityEngine;
using System.Collections;

public class ThrowerPlatform : MonoBehaviour
{
    [Tooltip("������ �̵� �ð�")]
    public float parabolicMoveDuration = 1.5f;

    [Tooltip("������ �ְ� ����")]
    public float parabolicHeight = 5f;

    [Tooltip("�߻��� ���� �ö� ť�긦 ������ ���̾�")]
    public LayerMask throwableLayer;

    [Tooltip("�߻��� ���� ť�긦 ������ ���� ũ�� (�ڽ� ũ��)")]
    public Vector3 detectionBoxSize = new Vector3(1f, 0.5f, 1f);

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Vector3 boxCenter = transform.position + Vector3.up * 0.5f;
        Gizmos.matrix = Matrix4x4.TRS(boxCenter, transform.rotation, detectionBoxSize);
        Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
    }

    public void ThrowCubes()
    {
        Vector3 boxCenter = transform.position + Vector3.up * 0.5f;
        Collider[] colliders = Physics.OverlapBox(boxCenter, detectionBoxSize * 0.5f, transform.rotation, throwableLayer);

        foreach (Collider col in colliders)
        {
            Rigidbody rb = col.attachedRigidbody;
            if (rb != null)
            {
                Vector3 startPos = rb.position;
                Vector3 endPos = startPos + transform.forward * 20f + Vector3.down * 10f;

                StartCoroutine(MoveAlongParabola(rb, startPos, endPos, parabolicHeight, parabolicMoveDuration));
            }
        }
    }

    private IEnumerator MoveAlongParabola(Rigidbody rb, Vector3 start, Vector3 end, float height, float duration)
    {
        Collider objCol = rb.GetComponent<Collider>();
        Collider platformCol = GetComponent<Collider>();

        // �浹 ���� ����
        if (objCol != null && platformCol != null)
            Physics.IgnoreCollision(objCol, platformCol, true);

        // Rigidbody ���� ����
        rb.isKinematic = true;

        // ��¦ ���� ��� �÷��� �浹 �̽� ����
        Vector3 liftedStart = start + Vector3.up * 0.2f;
        rb.MovePosition(liftedStart);
        yield return null;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;

            // ������ ���
            Vector3 mid = Vector3.Lerp(start, end, t);
            float parabola = 4 * height * t * (1 - t);
            mid.y = Mathf.Lerp(start.y, end.y, t) + parabola;

            rb.MovePosition(mid);
            elapsed += Time.deltaTime;
            yield return null;
        }

        rb.MovePosition(end);
        rb.isKinematic = false;

        // �浹 �ٽ� ���
        if (objCol != null && platformCol != null)
            Physics.IgnoreCollision(objCol, platformCol, false);
    }
}
