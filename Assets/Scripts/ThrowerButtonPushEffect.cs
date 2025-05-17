using UnityEngine;
using System.Collections;

public class ThrowerButtonPushEffect : MonoBehaviour
{
    [Tooltip("눌림 거리 (단위: 유니티 거리 단위)")]
    public float pushDistance = 0.05f;

    [Tooltip("눌리는 속도")]
    public float pushSpeed = 5f;

    [Tooltip("눌림 방향 (선택 사항). 비워두면(0) 자동으로 뒤 방향 사용)")]
    public Vector3 customPushDirection = Vector3.zero;

    [Tooltip("눌린 후 원래 위치로 돌아가기 전 대기 시간 (초)")]
    public float holdDuration = 0.1f;

    [Tooltip("눌릴 때 바뀔 색상")]
    public Color pressedColor = Color.red;

    private Vector3 _originalPosition;
    private Vector3 _targetPosition;
    private bool _isAnimating = false;

    private Renderer _renderer;
    private Color _originalColor;

    private void Start()
    {
        _originalPosition = transform.localPosition;

        Vector3 pushDir = customPushDirection == Vector3.zero
            ? -transform.forward
            : customPushDirection.normalized;

        _targetPosition = _originalPosition + pushDir * pushDistance;

        // 색상 관련 초기화
        _renderer = GetComponent<Renderer>();
        if (_renderer != null)
        {
            _originalColor = _renderer.material.color;
        }
    }

    public void StartPushEffect()
    {
        if (_isAnimating) return;

        StopAllCoroutines();
        StartCoroutine(PushAndReturn());
    }

    private IEnumerator PushAndReturn()
    {
        _isAnimating = true;

        float duration = 0.2f;
        float elapsed = 0f;

        Vector3 startPos = transform.localPosition;

        // 색상 변경
        if (_renderer != null)
        {
            _renderer.material.color = pressedColor;
        }

        // 1. 눌리기
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            transform.localPosition = Vector3.Lerp(startPos, _targetPosition, t);
            yield return null;
        }
        transform.localPosition = _targetPosition;

        // 2. 대기
        yield return new WaitForSeconds(holdDuration);

        // 3. 복귀
        elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            transform.localPosition = Vector3.Lerp(_targetPosition, _originalPosition, t);
            yield return null;
        }
        transform.localPosition = _originalPosition;

        // 색상 복귀
        if (_renderer != null)
        {
            _renderer.material.color = _originalColor;
        }

        _isAnimating = false;
    }
}
