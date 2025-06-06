using UnityEngine;

public class MainMenuCamera : MonoBehaviour
{
    [Header("카메라 설정")]
    public Camera targetCamera;
    public GameObject targetObject;  // 따라갈 대상 오브젝트
    private Transform target;        // 따라갈 대상의 Transform

    [Header("카메라 이동 설정")]
    public float smoothSpeed = 5.0f;
    public Vector3 offset = new Vector3(0, 2, -2.5f);

    [Header("수동 조작 설정")]
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
        // 카메라 설정
        if (targetCamera == null)
        {
            Debug.LogWarning("카메라가 설정되지 않았습니다. 새 카메라를 생성합니다.");
            GameObject cameraObj = new GameObject("CustomFollowCamera");
            targetCamera = cameraObj.AddComponent<Camera>();
            cameraTransform = cameraObj.transform;
        }
        else
        {
            cameraTransform = targetCamera.transform;
        }

        // 타겟 설정 (Transform 가져오기)
        if (targetObject != null)
        {
            target = targetObject.GetComponent<Transform>();
        }
        else
        {
            Debug.LogWarning("타겟 오브젝트가 설정되지 않았습니다. Inspector에서 지정해주세요.");
        }

        currentZoomDistance = -offset.z;
        targetCamera.enabled = true;
    }

    private Vector3 velocity = Vector3.zero;
    public float snapThresholdSpeed = 10.0f; // 이 속도 이상이면 즉시 따라감

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

        // 캐릭터 속도 계산
        float targetSpeed = (target.position - lastTargetPosition).magnitude / Time.deltaTime;

        if (targetSpeed > snapThresholdSpeed)
        {
            // 빠르게 움직이면 즉시 따라감 (Snap)
            cameraTransform.position = desiredPosition;
        }
        else
        {
            // 느릴 때는 부드럽게 따라감
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
