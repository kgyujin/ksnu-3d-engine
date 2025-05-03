using UnityEngine;

public class Door : MonoBehaviour
{
    private Vector3 originalPosition;
    private float moveY = 10f;
    private Vector3 targetPosition;
    public float doorMoveSpeed = 2f;
    private Transform doorTransform;

    void Awake()
    {
        doorTransform = transform;
        originalPosition = doorTransform.position;
        targetPosition = originalPosition;
    }

    public void Open()
    {
        targetPosition = new Vector3(doorTransform.position.x, doorTransform.position.y + moveY, doorTransform.position.z);
    }

    public void Close()
    {
        targetPosition = originalPosition;
    }

    void Update()
    {
        doorTransform.position = Vector3.Lerp(doorTransform.position, targetPosition, Time.deltaTime * doorMoveSpeed);
    }
}
