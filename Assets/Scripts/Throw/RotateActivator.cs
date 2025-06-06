using UnityEngine;

public class RotateActivator : MonoBehaviour, IButtonControllable
{
    public float baseSpeed = 10f;       // 초기 속도
    public float speedIncrement = 5f;   // 매 호출마다 증가할 속도
    public float maxSpeed = 100f;       // 최대 속도 제한

    private float currentSpeed = 0f;

    private void OnEnable()
    {
        currentSpeed = baseSpeed;
    }

    public void Increase()
    {
        RotateClockwise();
        IncreaseSpeed();
    }

    public void Decrease()
    {
        RotateCounterClockwise();
        IncreaseSpeed();
    }

    private void RotateClockwise()
    {
        transform.Rotate(Vector3.up, currentSpeed * Time.deltaTime);
    }

    private void RotateCounterClockwise()
    {
        transform.Rotate(Vector3.up, -currentSpeed * Time.deltaTime);
    }

    private void IncreaseSpeed()
    {
        currentSpeed = Mathf.Min(currentSpeed + speedIncrement, maxSpeed);
    }

    public void ResetRotationSpeed()
    {
        currentSpeed = baseSpeed;
    }
}
