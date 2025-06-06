using UnityEngine;

public class MainMenuCamera : MonoBehaviour
{
    [Header("ī�޶� ����")]
    public Camera targetCamera;
    public GameObject targetObject;  // ���� ��� ������Ʈ
    private Transform target;        // ���� ����� Transform

    [Header("ī�޶� �̵� ����")]
    public float smoothSpeed = 5.0f;
    public Vector3 offset = new Vector3(0, 2, -2.5f);

    [Header("���� ���� ����")]
    public bool enableManualControl = true;
    public float manualRotationSpeed = 100.0f;
    public float zoomSpeed = 5.0f;
    public float minZoomDistance = 1.0f;
    public float maxZoomDistance = 10.0f;

    private float currentZoomDistance;
    private float currentRotationY = 0f;
    private Transform cameraTransform;

    private void Start()
    {
        // ī�޶� ����
        if (targetCamera == null)
        {
            Debug.LogWarning("ī�޶� �������� �ʾҽ��ϴ�. �� ī�޶� �����մϴ�.");
            GameObject cameraObj = new GameObject("CustomFollowCamera");
            targetCamera = cameraObj.AddComponent<Camera>();
            cameraTransform = cameraObj.transform;
        }
        else
        {
            cameraTransform = targetCamera.transform;
        }

        // Ÿ�� ���� (Transform ��������)
        if (targetObject != null)
        {
            target = targetObject.GetComponent<Transform>();
        }
        else
        {
            Debug.LogWarning("Ÿ�� ������Ʈ�� �������� �ʾҽ��ϴ�. Inspector���� �������ּ���.");
        }

        currentZoomDistance = -offset.z;
        targetCamera.enabled = true;
    }

    private Vector3 velocity = Vector3.zero;
    public float snapThresholdSpeed = 10.0f; // �� �ӵ� �̻��̸� ��� ����

    private Vector3 lastTargetPosition;

    private void LateUpdate()
    {
        if (target == null) return;

        if (enableManualControl)
        {
            HandleManualControl();
        }

        Vector3 newOffset = new Vector3(0, offset.y, -currentZoomDistance);
        Quaternion rotation = Quaternion.Euler(25, currentRotationY, 0);
        newOffset = rotation * newOffset;

        Vector3 desiredPosition = target.position + newOffset;

        // ĳ���� �ӵ� ���
        float targetSpeed = (target.position - lastTargetPosition).magnitude / Time.deltaTime;

        if (targetSpeed > snapThresholdSpeed)
        {
            // ������ �����̸� ��� ���� (Snap)
            cameraTransform.position = desiredPosition;
        }
        else
        {
            // ���� ���� �ε巴�� ����
            cameraTransform.position = Vector3.SmoothDamp(
                cameraTransform.position,
                desiredPosition,
                ref velocity,
                1f / smoothSpeed
            );
        }

        cameraTransform.LookAt(target.position);
        lastTargetPosition = target.position;
    }




    private void HandleManualControl()
    {
        if (Input.GetMouseButton(1))
        {
            float rotationInput = Input.GetAxis("Mouse X") * manualRotationSpeed * Time.deltaTime;
            currentRotationY += rotationInput;
        }

        float zoomInput = Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
        currentZoomDistance = Mathf.Clamp(currentZoomDistance - zoomInput, minZoomDistance, maxZoomDistance);
    }
}
