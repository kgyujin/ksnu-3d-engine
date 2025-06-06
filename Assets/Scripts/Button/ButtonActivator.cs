using UnityEngine;

public class ButtonActivator : MonoBehaviour, IInteractable
{
    [Tooltip("버튼을 누르면 활성화할 오브젝트들")]
    public GameObject[] targetObjects;

    private bool isPressed = false;

    private Renderer _renderer;
    private Color _originalColor;
    private Color _pressedColor = Color.red;

    private ButtonPushEffect _pushEffect;

    public bool IsPressed => isPressed;

    private void Awake()
    {
        // 컴포넌트 참조
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

        // 색상 변경
        if (_renderer != null)
        {
            _renderer.material.color = _pressedColor;
        }

        // 눌림 효과 실행
        if (_pushEffect != null)
        {
            _pushEffect.StartPushEffect();
        }

        // 대상 오브젝트 활성화
        foreach (GameObject obj in targetObjects)
        {
            if (obj != null)
            {
                obj.SetActive(true);
            }
        }
    }
}
