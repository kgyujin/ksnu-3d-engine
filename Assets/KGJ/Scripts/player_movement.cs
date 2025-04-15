using UnityEngine;

public class player_movement : MonoBehaviour
{
    public Rigidbody player;
    public float speed = 5f;           // 이동 속도
    public float max_speed = 10f;      // 최대 속도
    public float jump_force = 5f;      // 점프 높이

    public float RaycastDistance = 1.2f; // 바닥 체크용 레이 거리
    public bool isGrounded;              // 바닥 상태
    public float slopeAngle;

    RaycastHit hit;

    private Camera_test cameras;
    private Vector3 movement;
    private bool jumpRequested;

    void Start()
    {
        player = GetComponent<Rigidbody>();
        cameras = GetComponent<Camera_test>();
    }

    void Update()
    {
        movement = cameras.moveDir.normalized;
        HandleRotation();
        // 점프 입력 처리만 Update에서 수행
        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            jumpRequested = true;
        }
    }

    private void FixedUpdate()
    {
        // 📌 moveDir은 FixedUpdate에서 가져오기 (카메라가 먼저 업데이트되도록 보장)
        

        // 계속해서 바닥에 닿았는지 확인
        isGrounded = Check_isGrounded();

        HandleMovement();
        HandleJump(); 
        CheckSlope();
    }

    private void HandleMovement()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out hit, RaycastDistance))
        {
            float angle = Vector3.Angle(hit.normal, Vector3.up);
            Vector3 slopeDir = Vector3.ProjectOnPlane(movement, hit.normal).normalized;

            float slopeBoost = 1f + Mathf.Sin(angle * Mathf.Deg2Rad); // 경사면 힘 보정
            Vector3 finalForce = slopeDir * speed * slopeBoost;

            player.AddForce(finalForce, ForceMode.Acceleration);
        }

        else
        {
            // 공중에 있을 때 이동
            Vector3 playerVelocity = player.linearVelocity;
            Vector2 horizontalVelocity = new Vector2(playerVelocity.x, playerVelocity.z);

            if (horizontalVelocity.magnitude < max_speed)
            {
                // 공중에서는 이동력을 조금 줄임
                float airControl = 0.7f;
                player.AddForce(movement * speed * airControl, ForceMode.Acceleration);
            }
        }

        // 최대 속도 보정
        Vector3 velocity = player.linearVelocity;
        Vector2 horizontalSpeed = new Vector2(velocity.x, velocity.z);

        if (horizontalSpeed.magnitude > max_speed)
        {
            // 최대 속도를 초과하면 보정
            horizontalSpeed = horizontalSpeed.normalized * max_speed;
            player.linearVelocity = new Vector3(horizontalSpeed.x, player.linearVelocity.y, horizontalSpeed.y);
        }

        // 마찰 처리 (감속)
        if (movement.magnitude < 0.1f && isGrounded)
        {
            Vector3 currentVelocity = player.linearVelocity;
            Vector2 currentHorizontalVelocity = new Vector2(currentVelocity.x, currentVelocity.z);

            if (currentHorizontalVelocity.magnitude < 0.5f)
            {
                // 거의 멈춘 상태면 완전히 정지
                player.linearVelocity = new Vector3(0f, player.linearVelocity.y, 0f);
            }
            else
            {
                // 아직 움직이는 중이면 감속
                float dampingFactor = 0.9f;
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
            Debug.DrawRay(hit.point, hit.normal, Color.green);
        }
    }

    private bool Check_isGrounded()
    {
        // 레이를 플레이어의 위치보다 약간 아래에서 시작하도록 수정
        if (Physics.Raycast(transform.position - Vector3.up * 0.1f, Vector3.down, out hit, RaycastDistance))
        {
            return true;
        }
        return false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (Check_isGrounded())
        {
            isGrounded = true;
        }
    }

    private void HandleRotation()
    {
        if (movement.magnitude > 0.1f)  // 이동이 있을 때만 회전
        {
            // 이동 방향을 바라보는 회전 계산 (y값을 0으로 고정)
            Vector3 flatMovement = new Vector3(movement.x, 0, movement.z);  // y축을 고정하여 회전
            Quaternion targetRotation = Quaternion.LookRotation(flatMovement);

            // 회전 속도에 맞게 부드럽게 회전
            float step = 180f * Time.deltaTime;  // 회전 속도 계산
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, step);
        }
        else
        {
            // 이동이 없을 경우에도 카메라의 방향을 반영하여 캐릭터 회전 (y값을 0으로 고정)
            Vector3 flatForward = new Vector3(cameras.cameraTransform.forward.x, 0f, cameras.cameraTransform.forward.z); // y축 고정
            Quaternion targetRotation = Quaternion.LookRotation(flatForward);

            float step = 360f * Time.deltaTime;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, step);
        }
    }
}
