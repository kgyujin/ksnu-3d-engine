using UnityEngine;

public class Door : MonoBehaviour
{
    private Vector3 originalPosition;
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
        targetPosition = new Vector3(doorTransform.position.x, 15f, doorTransform.position.z);
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
