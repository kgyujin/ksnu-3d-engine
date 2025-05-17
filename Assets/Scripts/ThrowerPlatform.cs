using UnityEngine;
using System.Collections;

public class ThrowerPlatform : MonoBehaviour
{
    [Tooltip("포물선 이동 시간")]
    public float parabolicMoveDuration = 1.5f;

    [Tooltip("포물선 최고 높이")]
    public float parabolicHeight = 5f;

    [Tooltip("발사판 위에 올라간 큐브를 감지할 레이어")]
    public LayerMask throwableLayer;

    [Tooltip("발사판 위의 큐브를 감지할 영역 크기 (박스 크기)")]
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

        // 충돌 무시 설정
        if (objCol != null && platformCol != null)
            Physics.IgnoreCollision(objCol, platformCol, true);

        // Rigidbody 물리 제거
        rb.isKinematic = true;

        // 살짝 위로 들어 올려서 충돌 이슈 방지
        Vector3 liftedStart = start + Vector3.up * 0.2f;
        rb.MovePosition(liftedStart);
        yield return null;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;

            // 포물선 계산
            Vector3 mid = Vector3.Lerp(start, end, t);
            float parabola = 4 * height * t * (1 - t);
            mid.y = Mathf.Lerp(start.y, end.y, t) + parabola;

            rb.MovePosition(mid);
            elapsed += Time.deltaTime;
            yield return null;
        }

        rb.MovePosition(end);
        rb.isKinematic = false;

        // 충돌 다시 허용
        if (objCol != null && platformCol != null)
            Physics.IgnoreCollision(objCol, platformCol, false);
    }
}
