//using UnityEngine;

//public class player_movement : MonoBehaviour
//{
//    public Rigidbody player;
//    public float speed = 5f;           // 이동 속도
//    public float max_speed = 10f;      // 최대 속도
//    public float jump_force = 5f;      // 점프 높이

//    [Header("Slope Settings")]
//    public float maxSlopeAngle = 45f;        // 올라갈 수 있는 최대 경사 각도
//    public float slopeForceMultiplier = 0.1f; // 경사면 힘 배율
//    public float diagonalSlopeBoost = 0.1f;   // 대각선 이동시 추가 보정값

//    [Header("Ground Check")]
//    public float RaycastDistance = 1.2f;     // 바닥 체크용 레이 거리
//    public bool isGrounded;                  // 바닥 상태
//    public float slopeAngle;                 // 현재 경사 각도

//    RaycastHit hit;

//    private Camera_test cameras;
//    private Vector3 movement;
//    private Vector3 inputDir;          // 원시 입력 방향
//    private Vector3 slopeMovementDir;  // 경사면에 조정된 이동 방향
//    private bool jumpRequested;
//    private bool isOnSlope;            // 경사면 위에 있는지 여부

//    void Start()
//    {
//        player = GetComponent<Rigidbody>();
//        cameras = GetComponent<Camera_test>();

//        // 중력 영향을 더 강하게 설정 (경사면 미끄러짐 방지)
//        player.constraints = RigidbodyConstraints.FreezeRotation;
//    }

//    void Update()
//    {
//        // 원시 입력 처리
//        inputDir = new Vector3(cameras.horizontalInput, 0, cameras.verticalInput);

//        // 이동 방향 (카메라 기준으로 변환)
//        movement = cameras.moveDir;

//        // 점프 입력 처리
//        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
//        {
//            jumpRequested = true;
//        }
//    }

//    private void FixedUpdate()
//    {
//        // 바닥 상태 확인
//        isGrounded = Check_isGrounded();

//        // 경사면 확인
//        CheckSlope();

//        // 이동 처리
//        HandleMovement();

//        // 회전 처리
//        HandleRotation();

//        // 점프 처리
//        HandleJump();
//    }

//    private void HandleMovement()
//    {
//        // 입력이 없으면 감속만 처리
//        if (movement.magnitude < 0.1f)
//        {
//            ApplyFriction();
//            return;
//        }

//        if (isGrounded)
//        {
//            if (isOnSlope && slopeAngle <= maxSlopeAngle)
//            {
//                // 경사면에서의 이동 처리
//                MovementOnSlope();
//            }
//            else
//            {
//                // 평지에서의 이동 처리
//                MovementOnFlat();
//            }
//        }
//        else
//        {
//            // 공중에서의 이동 처리
//            MovementInAir();
//        }

//        // 최대 속도 제한
//        LimitMaxSpeed();
//    }

//    private void MovementOnSlope()
//    {
//        // 경사면에 투영된 이동 벡터 계산
//        slopeMovementDir = Vector3.ProjectOnPlane(movement, hit.normal).normalized;

//        // 경사 각도에 따른 추가 힘 계산
//        float slopeMultiplier = 1.0f + (slopeAngle / maxSlopeAngle) * slopeForceMultiplier;

//        // 대각선 이동 감지 (전진/후진 + 좌/우)
//        if (Mathf.Abs(cameras.verticalInput) > 0.1f && Mathf.Abs(cameras.horizontalInput) > 0.1f)
//        {
//            // 대각선 이동 시 추가 보정
//            slopeMultiplier *= diagonalSlopeBoost;

//            // 경사면 위쪽 방향으로 이동 보정 (경사 방향으로 더 강한 힘)
//            slopeMovementDir += hit.normal * 0.2f;
//            slopeMovementDir.Normalize();
//        }

//        // 최종 힘 적용
//        Vector3 force = slopeMovementDir * speed * slopeMultiplier;

//        // 경사면에서 중력에 의한 미끄러짐 방지를 위한 상향력
//        if (slopeAngle > 10f)
//        {
//            // 경사가 급할수록 더 많은 상향력 적용
//            force += hit.normal * (slopeAngle * 0.2f);
//        }

//        player.AddForce(force, ForceMode.Acceleration);
//    }

//    private void MovementOnFlat()
//    {
//        // 평지에서 일반 이동
//        Vector3 force = movement * speed;
//        player.AddForce(force, ForceMode.Acceleration);
//    }

//    private void MovementInAir()
//    {
//        // 공중에서는 조작성 감소
//        float airControl = 0.5f;
//        Vector3 force = movement * speed * airControl;
//        player.AddForce(force, ForceMode.Acceleration);
//    }

//    private void LimitMaxSpeed()
//    {
//        // 현재 수평 속도 계산
//        Vector3 velocity = player.linearVelocity;
//        Vector2 horizontalVelocity = new Vector2(velocity.x, velocity.z);

//        // 최대 속도 제한
//        if (horizontalVelocity.magnitude > max_speed)
//        {
//            horizontalVelocity = horizontalVelocity.normalized * max_speed;
//            player.linearVelocity = new Vector3(
//                horizontalVelocity.x,
//                player.linearVelocity.y,
//                horizontalVelocity.y
//            );
//        }
//    }

//    private void ApplyFriction()
//    {
//        // 입력이 없을 때 마찰력 적용 (감속)
//        if (isGrounded)
//        {
//            Vector3 currentVelocity = player.linearVelocity;
//            Vector2 horizontalVelocity = new Vector2(currentVelocity.x, currentVelocity.z);

//            if (horizontalVelocity.magnitude < 0.5f)
//            {
//                // 거의 멈춘 상태면 완전히 정지
//                player.linearVelocity = new Vector3(0f, player.linearVelocity.y, 0f);
//            }
//            else
//            {
//                // 감속 처리
//                float dampingFactor = 0.85f; // 더 빠른 감속 (기존 0.9f)
//                player.linearVelocity = new Vector3(
//                    currentVelocity.x * dampingFactor,
//                    currentVelocity.y,
//                    currentVelocity.z * dampingFactor
//                );
//            }
//        }
//    }

//    private void HandleJump()
//    {
//        if (jumpRequested && isGrounded)
//        {
//            player.AddForce(Vector3.up * jump_force, ForceMode.VelocityChange);
//            isGrounded = false;
//            jumpRequested = false;
//        }
//    }

//    private void CheckSlope()
//    {
//        if (Physics.Raycast(transform.position, Vector3.down, out hit, RaycastDistance))
//        {
//            // 경사 각도 계산
//            slopeAngle = Vector3.Angle(hit.normal, Vector3.up);
//            isOnSlope = slopeAngle != 0;

//            // 디버그 Ray 그리기
//            Debug.DrawRay(hit.point, hit.normal, Color.green);
//        }
//        else
//        {
//            isOnSlope = false;
//            slopeAngle = 0f;
//        }
//    }

//    private bool Check_isGrounded()
//    {
//        // 바닥 체크 - 더 짧은 Ray와 스피어캐스트 조합
//        Vector3 origin = transform.position - Vector3.up * 0.1f;

//        // Raycast로 먼저 체크
//        if (Physics.Raycast(origin, Vector3.down, out hit, RaycastDistance - 0.1f))
//        {
//            return true;
//        }

//        // SphereCast로 추가 체크 (더 정확한 바닥 감지)
//        float radius = 0.3f;
//        return Physics.SphereCast(origin, radius, Vector3.down, out hit, RaycastDistance - 0.1f - radius);
//    }

//    private void HandleRotation()
//    {
//        if (movement.magnitude > 0.1f)
//        {
//            // 이동 방향을 바라보도록 회전
//            Quaternion targetRotation = Quaternion.LookRotation(movement);

//            // 더 빠른 회전 속도 (부드러운 이동을 위해)
//            float rotationSpeed = 15f;
//            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
//        }
//        else if (cameras.moveDir.magnitude > 0.1f)
//        {
//            // 이동이 없어도 카메라가 보는 방향으로 회전
//            Vector3 camForward = cameras.cameraTransform.forward;
//            camForward.y = 0f;
//            camForward.Normalize();

//            Quaternion targetRotation = Quaternion.LookRotation(camForward);
//            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
//        }
//    }

//    private void OnCollisionEnter(Collision collision)
//    {
//        if (Check_isGrounded())
//        {
//            isGrounded = true;
//        }
//    }
//}


//using UnityEngine;

//public class player_movement : MonoBehaviour
//{
//    public Rigidbody player;
//    public float speed = 5f;
//    public float max_speed = 10f;
//    public float jump_force = 5f;

//    [Header("Slope Settings")]
//    public float maxSlopeAngle = 45f;
//    public float slopeForceMultiplier = 0.01f;
//    // public float diagonalSlopeBoost = 0.1f; // 제거 또는 주석 처리

//    [Header("Ground Check")]
//    public float RaycastDistance = 1.2f;
//    public bool isGrounded;
//    public float slopeAngle;

//    RaycastHit hit;

//    private Camera_test cameras;
//    private Vector3 movement;
//    private Vector3 inputDir;
//    private Vector3 slopeMovementDir;
//    private bool jumpRequested;
//    private bool isOnSlope;

//    void Start()
//    {
//        player = GetComponent<Rigidbody>();
//        cameras = GetComponent<Camera_test>();

//        player.constraints = RigidbodyConstraints.FreezeRotation;
//    }

//    void Update()
//    {
//        inputDir = new Vector3(cameras.horizontalInput, 0, cameras.verticalInput);
//        movement = cameras.moveDir;

//        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
//        {
//            jumpRequested = true;
//        }
//    }

//    private void FixedUpdate()
//    {
//        isGrounded = Check_isGrounded();
//        CheckSlope();
//        HandleMovement();
//        HandleRotation();
//        HandleJump();
//    }

//    private void HandleMovement()
//    {
//        if (movement.magnitude < 0.1f)
//        {
//            ApplyFriction();
//            return;
//        }

//        if (isGrounded)
//        {
//            if (isOnSlope && slopeAngle <= maxSlopeAngle)
//            {
//                MovementOnSlope();
//            }
//            else
//            {
//                MovementOnFlat();
//            }
//        }
//        else
//        {
//            MovementInAir();
//        }

//        LimitMaxSpeed();
//    }

//    private void MovementOnSlope()
//    {
//        slopeMovementDir = Vector3.ProjectOnPlane(movement, hit.normal).normalized;

//        float slopeMultiplier = 1.0f + (slopeAngle / maxSlopeAngle) * slopeForceMultiplier;

//        // 대각선 보정 제거

//        // 미끄럼 방지 보정 (값 줄임)
//        if (slopeAngle > 10f)
//        {
//            slopeMultiplier += 0.05f;
//        }

//        Vector3 force = slopeMovementDir * speed * slopeMultiplier;

//        player.AddForce(force, ForceMode.Acceleration);

//        LimitMaxSpeed(); // 경사면에서도 속도 제한 보장
//    }

//    private void MovementOnFlat()
//    {
//        Vector3 force = movement * speed;
//        player.AddForce(force, ForceMode.Acceleration);
//    }

//    private void MovementInAir()
//    {
//        float airControl = 0.5f;
//        Vector3 force = movement * speed * airControl;
//        player.AddForce(force, ForceMode.Acceleration);
//    }

//    private void LimitMaxSpeed()
//    {
//        Vector3 velocity = player.linearVelocity;
//        Vector2 horizontalVelocity = new Vector2(velocity.x, velocity.z);

//        if (horizontalVelocity.magnitude > max_speed)
//        {
//            Vector2 limited = horizontalVelocity.normalized * max_speed;
//            player.linearVelocity = new Vector3(limited.x, velocity.y, limited.y);
//        }
//    }

//    private void ApplyFriction()
//    {
//        if (isGrounded)
//        {
//            Vector3 currentVelocity = player.linearVelocity;
//            Vector2 horizontalVelocity = new Vector2(currentVelocity.x, currentVelocity.z);

//            if (horizontalVelocity.magnitude < 0.5f)
//            {
//                player.linearVelocity = new Vector3(0f, currentVelocity.y, 0f);
//            }
//            else
//            {
//                float dampingFactor = 0.85f;
//                player.linearVelocity = new Vector3(
//                    currentVelocity.x * dampingFactor,
//                    currentVelocity.y,
//                    currentVelocity.z * dampingFactor
//                );
//            }
//        }
//    }

//    private void HandleJump()
//    {
//        if (jumpRequested && isGrounded)
//        {
//            player.AddForce(Vector3.up * jump_force, ForceMode.VelocityChange);
//            isGrounded = false;
//            jumpRequested = false;
//        }
//    }

//    private void CheckSlope()
//    {
//        if (Physics.Raycast(transform.position, Vector3.down, out hit, RaycastDistance))
//        {
//            slopeAngle = Vector3.Angle(hit.normal, Vector3.up);
//            isOnSlope = slopeAngle != 0;

//            Debug.DrawRay(hit.point, hit.normal, Color.green);
//        }
//        else
//        {
//            isOnSlope = false;
//            slopeAngle = 0f;
//        }
//    }

//    private bool Check_isGrounded()
//    {
//        Vector3 origin = transform.position - Vector3.up * 0.1f;

//        if (Physics.Raycast(origin, Vector3.down, out hit, RaycastDistance - 0.1f))
//        {
//            return true;
//        }

//        float radius = 0.3f;
//        return Physics.SphereCast(origin, radius, Vector3.down, out hit, RaycastDistance - 0.1f - radius);
//    }

//    private void HandleRotation()
//    {
//        if (movement.magnitude > 0.1f)
//        {
//            Quaternion targetRotation = Quaternion.LookRotation(movement);
//            float rotationSpeed = 15f;
//            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
//        }
//        else if (cameras.moveDir.magnitude > 0.1f)
//        {
//            Vector3 camForward = cameras.cameraTransform.forward;
//            camForward.y = 0f;
//            camForward.Normalize();

//            Quaternion targetRotation = Quaternion.LookRotation(camForward);
//            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
//        }
//    }

//    private void OnCollisionEnter(Collision collision)
//    {
//        if (Check_isGrounded())
//        {
//            isGrounded = true;
//        }
//    }
//}


using UnityEngine;

public class player_movement : MonoBehaviour
{
    public Rigidbody player;
    public float speed = 5f;
    public float max_speed = 10f;
    public float jump_force = 5f;

    [Header("Slope Settings")]
    public float maxSlopeAngle = 45f;
    public float slopeForceMultiplier = 0.1f;

    [Header("Ground Check")]
    public float RaycastDistance = 1.2f;
    public bool isGrounded;
    public float slopeAngle;

    private RaycastHit hit;

    private PlayerTrackingCamera cameras;
    private Vector3 movement;
    private Vector3 inputDir;
    private Vector3 slopeMovementDir;
    private bool jumpRequested;
    private bool isOnSlope;

    private Animator animator;

    Animator anim;

    void Start()
    {
        player = GetComponent<Rigidbody>();
        cameras = GetComponent<PlayerTrackingCamera>();
        player.constraints = RigidbodyConstraints.FreezeRotation;

        animator = GetComponent<Animator>();
    }

    void Update()
    {
        
        inputDir = new Vector3(cameras.horizontalInput, 0, cameras.verticalInput);
        movement = cameras.moveDir;


        float moveSpeed = new Vector2(player.linearVelocity.x, player.linearVelocity.z).magnitude;
        animator.SetFloat("MoveSpeed", moveSpeed / speed);

        //bool isGrounded = Physics.Raycast(transform.position, Vector3.down, 1.1f);
        isGrounded = Check_isGrounded();
        animator.SetBool("Grounded", isGrounded);

        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {

            jumpRequested = true;
        }
        if (isGrounded && Input.GetKeyDown(KeyCode.P))
        {
            //anim.SetBool("isJumping", true);
            animator.SetTrigger("Wave");
            //jumpRequested = true;
        }
    }

    private void FixedUpdate()
    {
        
        CheckSlope();
        HandleMovement();
        HandleRotation();
        HandleJump();
    }

    private void HandleMovement()
    {
        if (movement.magnitude < 0.1f)
        {
            ApplyFriction();
            return;
        }

        if (isGrounded)
        {
            if (isOnSlope && slopeAngle <= maxSlopeAngle)
            {
                MovementOnSlope();
            }
            else
            {
                MovementOnFlat();
            }
        }
        else
        {
            MovementInAir();
        }

        LimitMaxSpeed();
    }

    //private void MovementOnSlope()
    //{
    //    // 경사면에 투영된 이동 벡터 계산
    //    slopeMovementDir = Vector3.ProjectOnPlane(movement, hit.normal).normalized;

    //    // 경사 각도에 따른 힘 계산 - 수정된 부분
    //    // 내리막에서는 속도를 크게 줄임
    //    float slopeMultiplier;

    //    // 내리막 감지 (이동 방향과 경사면 법선의 내적이 양수면 내리막)
    //    bool isDownhill = Vector3.Dot(slopeMovementDir, Vector3.down) > 0;

    //    if (isDownhill)
    //    {
    //        // 내리막에서는 속도 크게 감소 (경사가 가파를수록 더 느리게)
    //        slopeMultiplier = 0.5f - (slopeAngle / maxSlopeAngle) * 0.3f;
    //    }
    //    else
    //    {
    //        // 오르막에서는 기존 로직과 유사하게 유지
    //        slopeMultiplier = 1.0f + (slopeAngle / maxSlopeAngle) * 0.05f;
    //    }

    //    // 대각선 이동 감지 (전진/후진 + 좌/우)
    //    if (Mathf.Abs(cameras.verticalInput) > 0.1f && Mathf.Abs(cameras.horizontalInput) > 0.1f)
    //    {
    //        // 대각선 이동 시 추가 보정
    //        float diagonalSlopeBoost = 0.8f; // 낮게 설정하여 속도 감소
    //        slopeMultiplier *= diagonalSlopeBoost;

    //        // 경사면 위쪽 방향으로 이동 보정 (경사 방향으로 더 강한 힘)
    //        slopeMovementDir += hit.normal * 0.2f;
    //        slopeMovementDir.Normalize();
    //    }

    //    // 최종 힘 적용
    //    Vector3 force = slopeMovementDir * speed * slopeMultiplier;

    //    // 경사면에서 중력에 의한 미끄러짐 방지를 위한 상향력
    //    if (slopeAngle > 10f)
    //    {
    //        // 경사가 급할수록 더 많은 상향력 적용
    //        force += hit.normal * (slopeAngle * 0.2f);
    //    }

    //    player.AddForce(force, ForceMode.Acceleration);
    //}
    //private void MovementOnSlope()
    //{
    //    // 경사면에 투영된 이동 벡터 계산
    //    slopeMovementDir = Vector3.ProjectOnPlane(movement, hit.normal).normalized;

    //    // 경사 각도에 따른 힘 계산
    //    // 내리막/오르막 감지 (이동 방향과 경사면 법선의 내적이 양수면 내리막)
    //    bool isDownhill = Vector3.Dot(slopeMovementDir, Vector3.down) > 0;
    //    float slopeMultiplier;

    //    if (isDownhill)
    //    {
    //        // 내리막에서는 속도 크게 감소 (경사가 가파를수록 더 느리게)
    //        slopeMultiplier = 0.5f - (slopeAngle / maxSlopeAngle) * 0.3f;
    //    }
    //    else
    //    {
    //        // 오르막에서는 경사에 따라 힘 증가 (대각선 이동을 고려하여 증가)
    //        slopeMultiplier = 1.0f + (slopeAngle / maxSlopeAngle) * 0.15f;
    //    }

    //    // 대각선 이동 감지 (전진/후진 + 좌/우)
    //    bool isDiagonal = Mathf.Abs(cameras.verticalInput) > 0.1f && Mathf.Abs(cameras.horizontalInput) > 0.1f;

    //    if (isDiagonal)
    //    {
    //        if (isDownhill)
    //        {
    //            // 내리막 대각선: 속도 감소 유지
    //            float diagonalSlopeBoost = 0.8f;
    //            slopeMultiplier *= diagonalSlopeBoost;
    //        }
    //        else
    //        {
    //            // 오르막 대각선: 추가 힘 부여
    //            float diagonalUphillBoost = 0.8f; // 오르막 대각선에서는 더 강한 힘
    //            slopeMultiplier *= diagonalUphillBoost;

    //            // 경사면 위쪽 방향으로 더 강한 힘을 가함
    //            slopeMovementDir += hit.normal * 0.5f;
    //            slopeMovementDir.Normalize();
    //        }
    //    }

    //    // 최종 힘 적용
    //    Vector3 force = slopeMovementDir * speed * slopeMultiplier;

    //    // 경사면에서 중력에 의한 미끄러짐 방지를 위한 상향력
    //    if (slopeAngle > 10f)
    //    {
    //        // 대각선 오르막에서는 추가 상향력 부여
    //        float upwardMultiplier = isDiagonal && !isDownhill ? 0.3f : 0.2f;
    //        force += hit.normal * (slopeAngle * upwardMultiplier);
    //    }

    //    player.AddForce(force, ForceMode.Acceleration);
    //}

    private void MovementOnSlope()
    {
        // 경사면에 투영된 이동 벡터 계산
        slopeMovementDir = Vector3.ProjectOnPlane(movement, hit.normal).normalized;

        // 15도 미만에서는 기존속도 유지
        if (slopeAngle < 15f)
        {
            // 기존 속도 유지 (정상 이동)
            Vector3 force1 = slopeMovementDir * speed;
            player.AddForce(force1, ForceMode.Acceleration);
            return;
        }

        // 내리막/오르막 감지 (이동 방향과 경사면 법선의 내적이 양수면 내리막)
        bool isDownhill = Vector3.Dot(slopeMovementDir, Vector3.down) > 0;
        float slopeMultiplier;

        if (isDownhill)
        {
            // 내리막에서는 속도 크게 감소 (경사가 가파를수록 더 느리게)
            slopeMultiplier = 0.5f - (slopeAngle / maxSlopeAngle) * 0.3f;
        }
        else
        {
            // 오르막에서는 경사에 따라 힘 증가 (대각선 이동을 고려하여 증가)
            slopeMultiplier = 1.0f + (slopeAngle / maxSlopeAngle) * 0.15f;
        }

        // 대각선 이동 감지 (전진/후진 + 좌/우)
        bool isDiagonal = Mathf.Abs(cameras.verticalInput) > 0.1f && Mathf.Abs(cameras.horizontalInput) > 0.1f;

        if (isDiagonal)
        {
            if (isDownhill)
            {
                // 내리막 대각선: 속도 감소 유지
                float diagonalSlopeBoost = 0.8f;
                slopeMultiplier *= diagonalSlopeBoost;
            }
            else
            {
                // 오르막 대각선: 추가 힘 부여
                float diagonalUphillBoost = 0.8f; // 오르막 대각선에서는 더 강한 힘
                slopeMultiplier *= diagonalUphillBoost;
                // 경사면 위쪽 방향으로 더 강한 힘을 가함
                slopeMovementDir += hit.normal * 0.5f;
                slopeMovementDir.Normalize();
            }
        }

        // 최종 힘 적용
        Vector3 force = slopeMovementDir * speed * slopeMultiplier;

        // 경사면에서 중력에 의한 미끄러짐 방지를 위한 상향력
        if (slopeAngle > 10f)
        {
            // 대각선 오르막에서는 추가 상향력 부여
            float upwardMultiplier = isDiagonal && !isDownhill ? 0.3f : 0.2f;
            force += hit.normal * (slopeAngle * upwardMultiplier);
        }

        player.AddForce(force, ForceMode.Acceleration);
    }

    private void MovementOnFlat()
    {
        Vector3 force = movement * speed;
        player.AddForce(force, ForceMode.Acceleration);
    }

    private void MovementInAir()
    {
        float airControl = 0.5f;
        Vector3 force = movement * speed * airControl;
        player.AddForce(force, ForceMode.Acceleration);
    }

    private void LimitMaxSpeed()
    {
        Vector3 velocity = player.linearVelocity;
        Vector2 horizontalVelocity = new Vector2(velocity.x, velocity.z);

        if (horizontalVelocity.magnitude > max_speed)
        {
            horizontalVelocity = horizontalVelocity.normalized * max_speed;
            player.linearVelocity = new Vector3(horizontalVelocity.x, player.linearVelocity.y, horizontalVelocity.y);
        }
    }

    private void ApplyFriction()
    {
        if (isGrounded)
        {
            Vector3 currentVelocity = player.linearVelocity;
            Vector2 horizontalVelocity = new Vector2(currentVelocity.x, currentVelocity.z);

            if (horizontalVelocity.magnitude < 0.5f)
            {
                player.linearVelocity = new Vector3(0f, currentVelocity.y, 0f);
            }
            else
            {
                float dampingFactor = 0.85f;
                player.linearVelocity = new Vector3(
                    currentVelocity.x * dampingFactor,
                    currentVelocity.y,
                    currentVelocity.z * dampingFactor
                );
            }
        }
    }

    private void HandleJump()
    {
        if (jumpRequested && isGrounded)
        {
            player.AddForce(Vector3.up * jump_force, ForceMode.VelocityChange);
            isGrounded = false;
            jumpRequested = false;
        }
    }

    private void CheckSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out hit, RaycastDistance))
        {
            slopeAngle = Vector3.Angle(hit.normal, Vector3.up);
            isOnSlope = slopeAngle != 0;
            Debug.DrawRay(hit.point, hit.normal, Color.green);
        }
        else
        {
            isOnSlope = false;
            slopeAngle = 0f;
        }
    }

    private bool Check_isGrounded()
    {
        Vector3 origin = transform.position - Vector3.up * 0.1f;

        if (Physics.Raycast(origin, Vector3.down, out hit, RaycastDistance - 0.1f,~0, QueryTriggerInteraction.Ignore))

        {
            return true;
        }

        float radius = 0.3f;
        //return Physics.SphereCast(origin, radius, Vector3.down, out hit, RaycastDistance - 0.1f - radius);
        return Physics.SphereCast(origin, radius, Vector3.down, out hit, RaycastDistance - 0.1f - radius,~0, QueryTriggerInteraction.Ignore);


    }

    //private bool Check_isGrounded()
    //{
    //    Vector3 origin = transform.position - Vector3.up * 0.1f;

    //    // Raycast 검사
    //    if (Physics.Raycast(origin, Vector3.down, out hit, RaycastDistance - 0.1f))
    //    {
    //        if (!hit.collider.isTrigger)
    //        {
    //            return true;
    //        }
    //    }

    //    // SphereCast 검사
    //    float radius = 0.3f;
    //    if (Physics.SphereCast(origin, radius, Vector3.down, out hit, RaycastDistance - 0.1f - radius))
    //    {
    //        if (!hit.collider.isTrigger)
    //        {
    //            return true;
    //        }
    //    }

    //    return false;
    //}



    private void HandleRotation()
    {
        if (movement.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(movement);
            float rotationSpeed = 15f;
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
        else if (cameras.moveDir.magnitude > 0.1f)
        {
            Vector3 camForward = cameras.cameraTransform.forward;
            camForward.y = 0f;
            camForward.Normalize();

            Quaternion targetRotation = Quaternion.LookRotation(camForward);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (Check_isGrounded())
        {
            isGrounded = true;
        }
    }
}
