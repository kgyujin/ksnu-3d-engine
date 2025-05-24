using System.Collections;
using UnityEngine;

public class ThrowerActivator : MonoBehaviour, IInteractable
{
    public GameObject[] targetObjects;

    [Header("회전 설정")]
    public float targetZRotation = 45f;

    [Header("절대 위치 설정 (로컬 좌표 기준)")]
    public Vector3 targetPosition;

    [Header("애니메이션 설정")]
    public float moveDuration = 0.1f;   // 빠르게 보이도록 짧게
    public float holdDuration = 0.2f;

    private bool isPressed = false;

    private Renderer _renderer;
    private Color _originalColor;
    private Color _pressedColor = Color.red;

    public ThrowerPlatformParents throwerPlatform;

    private ButtonPushEffect _pushEffect;

    public bool IsPressed => isPressed;

    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
        _pushEffect = GetComponent<ButtonPushEffect>();

        if (_renderer != null)
        {
            _originalColor = _renderer.material.color;
        }
    }

    public void Interact()
    {
        if (isPressed) return;

        isPressed = true;

        if (_renderer != null)
            _renderer.material.color = _pressedColor;

        if (_pushEffect != null)
            _pushEffect.StartPushEffect();

        StartCoroutine(RotateAllAndThrow());
    }

    private IEnumerator RotateAllAndThrow()
    {
        foreach (GameObject obj in targetObjects)
        {
            if (obj != null)
                yield return RotateAndMoveLerp(obj.transform, targetZRotation, targetPosition, moveDuration, holdDuration);
        }

        if (throwerPlatform != null)
        {
            throwerPlatform.LaunchObjects();
        }

        isPressed = false;

        if (_renderer != null)
            _renderer.material.color = _originalColor;
    }

    private IEnumerator RotateAndMoveLerp(Transform target, float newZAngle, Vector3 targetLocalPos, float duration, float waitTime)
    {
        Quaternion originalRot = target.rotation;
        Quaternion targetRot = Quaternion.Euler(target.eulerAngles.x, target.eulerAngles.y, newZAngle);

        Vector3 originalLocalPos = target.localPosition;

        Rigidbody rb = target.GetComponent<Rigidbody>();

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            Quaternion lerpedRot = Quaternion.Slerp(originalRot, targetRot, t);
            Vector3 lerpedLocalPos = Vector3.Lerp(originalLocalPos, targetLocalPos, t);

            if (rb != null && rb.isKinematic)
            {
                rb.MoveRotation(lerpedRot);
                rb.MovePosition(target.parent.TransformPoint(lerpedLocalPos)); // 로컬 → 월드
            }
            else
            {
                target.rotation = lerpedRot;
                target.localPosition = lerpedLocalPos;
            }

            yield return null;
        }

        if (rb != null && rb.isKinematic)
        {
            rb.MoveRotation(targetRot);
            rb.MovePosition(target.parent.TransformPoint(targetLocalPos));
        }
        else
        {
            target.rotation = targetRot;
            target.localPosition = targetLocalPos;
        }

        yield return new WaitForSeconds(waitTime);

        // 복원
        elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            Quaternion lerpedRot = Quaternion.Slerp(targetRot, originalRot, t);
            Vector3 lerpedLocalPos = Vector3.Lerp(targetLocalPos, originalLocalPos, t);

            if (rb != null && rb.isKinematic)
            {
                rb.MoveRotation(lerpedRot);
                rb.MovePosition(target.parent.TransformPoint(lerpedLocalPos));
            }
            else
            {
                target.rotation = lerpedRot;
                target.localPosition = lerpedLocalPos;
            }

            yield return null;
        }

        if (rb != null && rb.isKinematic)
        {
            rb.MoveRotation(originalRot);
            rb.MovePosition(target.parent.TransformPoint(originalLocalPos));
        }
        else
        {
            target.rotation = originalRot;
            target.localPosition = originalLocalPos;
        }
    }
}
