
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
