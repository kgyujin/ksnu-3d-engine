using UnityEngine;
using UnityEngine.EventSystems;

public class Camera_test : MonoBehaviour
{
    [Header("Camera Settings")]
    public Transform cameraTransform;                         // 메인 카메라 트랜스폼
    public Vector3 cameraOffset = new Vector3(0, 2, -5);      // 플레이어 기준 오프셋
    public float mouseSensitivity = 5f;                       // 마우스 감도
    public float pitchMin = -35f;                             // 아래 최대 각도
    public float pitchMax = 60f;                              // 위 최대 각도
    public float cameraSmoothTime = 0.15f;                    // 카메라 이동 부드럽기

    [Header("Movement Settings")]
    public float moveSpeed = 5f;

    [Header("References")]
    public Rigidbody playerRigidbody;                         // 따라갈 플레이어 Rigidbody

    private float yaw = 0f;
    private float pitch = 0f;

    private Vector3 currentVelocity = Vector3.zero;
    public Vector3 moveDir;

    void Start()
    {
        if (cameraTransform == null)
            cameraTransform = Camera.main.transform;

        if (playerRigidbody == null)
            playerRigidbody = GetComponent<Rigidbody>();

        // 초기 카메라 회전
        Vector3 angles = transform.eulerAngles;
        yaw = angles.y;
        pitch = angles.x;

        // 마우스 커서 숨기기
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        HandleCameraRotation();
        HandleMovementInput();
    }

    void FixedUpdate()
    {
        HandleCameraPosition();
    }

    /// <summary>
    /// 마우스로 카메라 회전 제어
    /// </summary>
    void HandleCameraRotation()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        yaw += mouseX;
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, pitchMin, pitchMax);
    }

    /// <summary>
    /// 카메라 위치를 부드럽게 플레이어에 맞춰 이동
    /// </summary>
    //void HandleCameraPosition()
    //{
    //    Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
    //    Vector3 desiredPosition = playerRigidbody.position + rotation * cameraOffset;

    //    cameraTransform.position = Vector3.Lerp(
    //        cameraTransform.position,
    //        desiredPosition,
    //        Time.fixedDeltaTime / cameraSmoothTime
    //    );

    //    cameraTransform.rotation = rotation;
    //    cameraTransform.LookAt(playerRigidbody.position + Vector3.up * 1.5f);
    //}

    void HandleCameraPosition()
    {
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);

        // 오프셋을 수직 성분과 수평 성분으로 분리
        Vector3 horizontalOffset = new Vector3(cameraOffset.x, 0, cameraOffset.z);
        Vector3 verticalOffset = new Vector3(0, cameraOffset.y, 0);

        // 수평 오프셋만 회전에 반영
        Vector3 rotatedHorizontal = rotation * horizontalOffset;

        // 최종 카메라 위치 계산
        Vector3 desiredPosition = playerRigidbody.position + verticalOffset + rotatedHorizontal;

        cameraTransform.position = Vector3.Lerp(
            cameraTransform.position,
            desiredPosition,
            Time.fixedDeltaTime / cameraSmoothTime
        );

        // 카메라가 플레이어를 바라보도록
        cameraTransform.rotation = rotation;
        cameraTransform.LookAt(playerRigidbody.position + Vector3.up * 1.5f);
    }


    /// <summary>
    /// 플레이어 이동 방향 계산 (카메라 기준)
    /// </summary>
    void HandleMovementInput()
    {
        //Vector3 camForward = cameraTransform.forward;
        //Vector3 camRight = cameraTransform.right;

        //camForward.y = 0f;
        //camRight.y = 0f;
        //camForward.Normalize();
        //camRight.Normalize();

        //float horizontal = Input.GetAxis("Horizontal");
        //float vertical = Input.GetAxis("Vertical");

        //moveDir = (camForward * vertical + camRight * horizontal).normalized;
        //if (moveDir.magnitude > 0.1f)
        //{
        //    Quaternion targetRotation = Quaternion.LookRotation(moveDir);
        //    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 360f);
        //}
        //float horizontal = Input.GetAxis("Horizontal");
        //float vertical = Input.GetAxis("Vertical");

        //// 플레이어 기준 방향으로 movement 계산
        //Vector3 forward = transform.forward;
        //Vector3 right = transform.right;

        //moveDir = (forward * vertical + right * horizontal).normalized;

        //// 카메라 y축 회전을 따라 플레이어 회전
        //if (moveDir.magnitude > 0.1f)
        //{
        //    Vector3 cameraForward = cameraTransform.forward;
        //    cameraForward.y = 0f;
        //    cameraForward.Normalize();

        //    Quaternion targetRotation = Quaternion.LookRotation(cameraForward);
        //    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        //}

        //float horizontal = Input.GetAxis("Horizontal");
        //float vertical = Input.GetAxis("Vertical");

        //// 카메라 기준 이동 방향 계산
        //Vector3 camForward = cameraTransform.forward;
        //Vector3 camRight = cameraTransform.right;

        //camForward.y = 0f;
        //camRight.y = 0f;
        //camForward.Normalize();
        //camRight.Normalize();

        //moveDir = (camForward * vertical + camRight * horizontal).normalized;

        //// 이동 방향을 바라보도록 회전
        //if (moveDir.magnitude > 0.1f)
        //{
        //    Quaternion targetRotation = Quaternion.LookRotation(moveDir);
        //    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        //}
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // 카메라 forward, right를 수평면에 투영
        Vector3 camForward = Vector3.ProjectOnPlane(cameraTransform.forward, Vector3.up).normalized;
        Vector3 camRight = Vector3.ProjectOnPlane(cameraTransform.right, Vector3.up).normalized;

        // 카메라 기준 이동 벡터 계산
        moveDir = (camForward * vertical + camRight * horizontal).normalized;

        // 이동 방향으로 회전
        if (moveDir.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }
    }
}
