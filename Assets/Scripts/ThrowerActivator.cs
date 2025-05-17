using System.Collections;
using UnityEngine;

public class ThrowerActivator : MonoBehaviour, IInteractable
{
    [Tooltip("버튼을 누르면 회전시킬 오브젝트들")]
    public GameObject[] targetObjects;

    [Tooltip("회전시 적용할 X축 각도 (절대값)")]
    public float targetXRotation = 45f;

    [Tooltip("회전 애니메이션 지속 시간 (초)")]
    public float rotateDuration = 0.3f;

    [Tooltip("회전 후 대기 시간 (초)")]
    public float holdDuration = 0.2f;

    private bool isPressed = false;

    private Renderer _renderer;
    private Color _originalColor;
    private Color _pressedColor = Color.red;

    public ThrowerPlatform throwerPlatform;  // 인스펙터에서 연결

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

        foreach (GameObject obj in targetObjects)
        {
            if (obj != null)
                StartCoroutine(RotateAndReturn(obj.transform, targetXRotation, rotateDuration, holdDuration));
        }
    }

    private IEnumerator RotateAndReturn(Transform target, float newXAngle, float duration, float waitTime)
    {
        float originalX = target.eulerAngles.x;
        Quaternion originalRot = Quaternion.Euler(originalX, -90f, -90f);
        Quaternion targetRot = Quaternion.Euler(newXAngle, -90f, -90f);

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            target.rotation = Quaternion.Slerp(originalRot, targetRot, t);
            yield return null;
        }

        target.rotation = targetRot;

        yield return new WaitForSeconds(waitTime);

        elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            target.rotation = Quaternion.Slerp(targetRot, originalRot, t);
            yield return null;
        }

        target.rotation = originalRot;

        isPressed = false;

        if (_renderer != null)
            _renderer.material.color = _originalColor;

        if (throwerPlatform != null)
        {
            throwerPlatform.ThrowCubes();  // 투척은 여기서 전적으로 처리
        }
    }
}
