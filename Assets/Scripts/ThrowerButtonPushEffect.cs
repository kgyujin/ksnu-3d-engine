//using UnityEngine;
//using System.Collections;
//using System;

//public class ThrowerButtonPushEffect : MonoBehaviour
//{
//    [Header("눌림 설정")]
//    public float pushDistance = 0.05f;
//    public float pushSpeed = 5f;
//    public float holdDuration = 0.1f;
//    public Color pressedColor = Color.red;
//    public Vector3 customPushDirection = Vector3.zero;

//    private Vector3 _originalPosition;
//    private Vector3 _targetPosition;
//    private bool _isAnimating = false;

//    private Renderer _renderer;
//    private Color _originalColor;

//    // 애니메이션 완료 시 호출할 이벤트
//    public event Action OnPushComplete;

//    private void Start()
//    {
//        _originalPosition = transform.localPosition;

//        Vector3 pushDir = customPushDirection == Vector3.zero
//            ? -transform.forward
//            : customPushDirection.normalized;

//        _targetPosition = _originalPosition + pushDir * pushDistance;

//        _renderer = GetComponent<Renderer>();
//        if (_renderer != null)
//        {
//            _originalColor = _renderer.material.color;
//        }
//    }

//    public void StartPushEffect()
//    {
//        if (!_isAnimating)
//        {
//            StartCoroutine(PushAndReturn());
//        }
//    }

//    private IEnumerator PushAndReturn()
//    {
//        _isAnimating = true;

//        float duration = 1f / pushSpeed;
//        float elapsed = 0f;

//        Vector3 startPos = transform.localPosition;

//        if (_renderer != null)
//            _renderer.material.color = pressedColor;

//        while (elapsed < duration)
//        {
//            elapsed += Time.deltaTime;
//            float t = Mathf.Clamp01(elapsed / duration);
//            transform.localPosition = Vector3.Lerp(startPos, _targetPosition, t);
//            yield return null;
//        }



//        transform.localPosition = _targetPosition;

//        yield return new WaitForSeconds(holdDuration);

//        elapsed = 0f;
//        while (elapsed < duration && _isAnimating)
//        {
//            elapsed += Time.deltaTime;
//            float t = Mathf.Clamp01(elapsed / duration);
//            transform.localPosition = Vector3.Lerp(_targetPosition, _originalPosition, t);
//            yield return null;
//        }


//        transform.localPosition = _originalPosition;

//        if (_renderer != null)
//            _renderer.material.color = _originalColor;

//        _isAnimating = false;

//        // 애니메이션 완료 이벤트 호출
//        OnPushComplete?.Invoke();
//    }

//    public void HoldingButton()
//    {
//        _isAnimating = true;

//    }

//    public void ReleasButton()
//    {
//        _isAnimating = false;
//    }

//}

using UnityEngine;
using UnityEngine;
using System.Collections;
using System;

public class ThrowerButtonPushEffect : MonoBehaviour
{
    [Header("눌림 설정")]
    public float pushDistance = 0.05f;
    public float pushSpeed = 5f;
    public float holdDuration = 0.1f;
    public Color pressedColor = Color.red;
    public Vector3 customPushDirection = Vector3.zero;

    private Vector3 _originalPosition;
    private Vector3 _targetPosition;
    private bool _isAnimating = false;

    private Renderer _renderer;
    private Color _originalColor;

    // 애니메이션 완료 시 호출할 이벤트
    public event Action OnPushComplete;

    private void Start()
    {
        _originalPosition = transform.localPosition;

        Vector3 pushDir = customPushDirection == Vector3.zero
            ? -transform.forward
            : customPushDirection.normalized;

        _targetPosition = _originalPosition + pushDir * pushDistance;

        _renderer = GetComponent<Renderer>();
        if (_renderer != null)
        {
            _originalColor = _renderer.material.color;
        }
    }

    // 버튼이 눌렸을 때 효과를 시작합니다.
    public void StartPushEffect()
    {
        if (!_isAnimating)
        {
            StartCoroutine(PushAndReturn());
        }
    }

    // 눌렀을 때, 버튼이 눌리는 애니메이션
    private IEnumerator PushAndReturn()
    {
        _isAnimating = true;

        float duration = 1f / pushSpeed;
        float elapsed = 0f;

        Vector3 startPos = transform.localPosition;

        if (_renderer != null)
            _renderer.material.color = pressedColor;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            transform.localPosition = Vector3.Lerp(startPos, _targetPosition, t);
            yield return null;
        }

        transform.localPosition = _targetPosition;

        // 잠시 기다린 후, 돌아오는 애니메이션을 시작합니다.
        yield return new WaitForSeconds(holdDuration);

        elapsed = 0f;
        while (elapsed < duration && _isAnimating)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            transform.localPosition = Vector3.Lerp(_targetPosition, _originalPosition, t);
            yield return null;
        }

        transform.localPosition = _originalPosition;

        if (_renderer != null)
            _renderer.material.color = _originalColor;

        _isAnimating = false;

        // 애니메이션 완료 이벤트 호출
        OnPushComplete?.Invoke();
    }

    // 버튼을 계속 눌렀을 때 호출되는 메서드
    public void HoldingButton()
    {
        // 애니메이션이 실행 중이 아니라면 새로 시작합니다.
        if (!_isAnimating)
        {
            StartPushEffect();
        }
    }

    // 버튼을 놓았을 때 호출되는 메서드
    public void ReleasButton()
    {
        _isAnimating = false;  // 애니메이션을 멈추고
        // 현재의 위치로 버튼을 돌아가게 할 수 있습니다.
        StopCoroutine("PushAndReturn");
        transform.localPosition = _originalPosition;

        if (_renderer != null)
        {
            _renderer.material.color = _originalColor;  // 원래 색으로 복귀
        }
    }
}
