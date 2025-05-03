using UnityEngine;

public class ButtonPushEffect : MonoBehaviour
{
    [Tooltip("눌림 거리 (단위: 유니티 거리 단위)")]
    public float pushDistance = 0.05f;

    [Tooltip("눌리는 속도")]
    public float pushSpeed = 5f;

    [Tooltip("눌림 방향 (선택 사항). 비워두면(0) 자동으로 뒤 방향 사용)")]
    public Vector3 customPushDirection = Vector3.zero;

    private Vector3 _originalPosition;
    private Vector3 _targetPosition;
    private bool _isPushed = false;

    private void Start()
    {
        _originalPosition = transform.localPosition;

        Vector3 pushDir = customPushDirection == Vector3.zero
            ? -transform.forward  // 자동으로 벽 반대 방향
            : customPushDirection.normalized;

        _targetPosition = _originalPosition + pushDir * pushDistance;
    }

    public void StartPushEffect()
    {
        if (_isPushed) return;

        _isPushed = true;
        StopAllCoroutines();
        StartCoroutine(PushSmoothly());
    }

    private System.Collections.IEnumerator PushSmoothly()
    {
        float elapsed = 0f;
        float duration = 0.2f;

        Vector3 startPos = transform.localPosition;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            transform.localPosition = Vector3.Lerp(startPos, _targetPosition, t);
            yield return null;
        }

        transform.localPosition = _targetPosition;
    }
}
