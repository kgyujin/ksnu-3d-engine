using UnityEngine;

public class RaycastObjectMover : MonoBehaviour
{
    [Header("감지 설정")]
    public LayerMask selectableLayer; // 선택 가능한 오브젝트 레이어
    public LayerMask Wand;
    public float rayDistance = 5f; // 레이 길이
    public float moveForce = 500f; // 이동 힘 세기
    public bool drawDebugRay = true; // 디버그용 레이 표시 여부

    [Header("회전 설정")]
    public float rotationSmoothTime = 0.1f; // 회전 부드러움 정도
    public bool followCameraRotationY = true; // 카메라 Y축 회전 따라가기

    private Transform selectedObject = null; // 선택된 오브젝트
    private Vector3 moveVelocity = Vector3.zero; // 스무스 이동에 사용될 속도 벡터
    private float currentRotationVelocity; // 회전 스무스 댐핑에 사용될 변수

    private Outline lastOutline = null; // 하이라이트(감지)된 오브젝트의 Outline
    private Outline selectedOutline = null; // 선택(잡기)된 오브젝트의 Outline

    private Renderer selectedRenderer = null; // 선택된 오브젝트의 렌더러
    private Rigidbody selectedRigidbody = null; // 선택된 오브젝트의 리지드바디

    private Camera cam; // 메인 카메라

    private Rigidbody playerRigidbody = null; // 플레이어 리지드바디

    private Collider playerCollider = null; // 플레이어 콜라이더
    private Collider objectCollider = null; // 오브젝트 콜라이더

    private bool wasKinematic = false; // 원래 키네마틱 여부 저장
    private bool hadGravity = false; // 원래 중력 여부 저장

    // 선택 시 초기 회전값 저장
    private Quaternion originalRotation;
    // 카메라와 오브젝트 간의 초기 Y축 회전 차이 저장
    private float initialYRotationOffset;
    // 초기 카메라 Y축 각도
    private float initialCameraYRotation;

    // 오브젝트 선택 지점에 대한 오프셋 저장
    private Vector3 grabOffset = Vector3.zero;

    // 카메라로부터의 상대 거리 저장
    private float grabDistance = 0f;

    // 카메라 기준 상대 방향 저장
    private Vector3 grabbedLocalPosition;

    void Start()
    {
        cam = Camera.main; // 메인 카메라 참조

        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            playerCollider = player.GetComponent<Collider>();
            playerRigidbody = player.GetComponent<Rigidbody>();
        }
    }

    void Update()
    {
        if (cam == null) return;

        // 카메라 중앙 기준으로 레이 쏘기
        Ray ray = cam.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));

        if (drawDebugRay)
            Debug.DrawRay(ray.origin, ray.direction * rayDistance, Color.cyan);

        // 마우스 휠로 레이 거리 조절
        if (selectedObject == null)  // 오브젝트가 선택되지 않은 경우에만 거리 조절
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            rayDistance += scroll * 5f;
            rayDistance = Mathf.Clamp(rayDistance, 1f, 100f);
        }
        else
        {
            // 오브젝트가 선택된 상태에서 마우스 휠로 거리 조절
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (Mathf.Abs(scroll) > 0.01f)
            {
                grabDistance -= scroll * 5f;
                grabDistance = Mathf.Clamp(grabDistance, 1f, 100f);
            }
        }

        // 오브젝트를 선택하지 않았을 때만 감지 및 선택 처리
        if (selectedObject == null)
        {
            HandleHighlighting(ray);
            HandleSelection(ray);
        }

        // 우클릭으로 놓기
        if (Input.GetMouseButtonDown(1))
        {
            ReleaseSelectedObject();
        }

        //effect();
    }

    public GameObject wand;
    Vector3 targetPos;

    void effect()
    {

        Wand wand = GetComponent<Wand>();
        Vector3 wand_position = wand.transform.position;
        Debug.DrawLine(wand_position, targetPos, Color.blue);
        Debug.Log("테스트");
    }

    void FixedUpdate()
    {
        // 오브젝트가 선택된 경우 카메라와의 상대 위치를 유지하며 이동
        if (selectedObject != null && selectedRigidbody != null)
        {
            // 카메라로부터의 상대 위치 계산
            Vector3 targetPos = cam.transform.position + cam.transform.forward * grabDistance +
                                cam.transform.right * grabbedLocalPosition.x +
                                cam.transform.up * grabbedLocalPosition.y - grabOffset;

            Vector3 direction = targetPos - selectedRigidbody.position;

            // 플레이어 속도 보정 적용
            Vector3 playerVelocity = playerRigidbody != null ? playerRigidbody.linearVelocity : Vector3.zero;
            Vector3 desiredVelocity = direction.normalized * Mathf.Min(direction.magnitude * 10f, moveForce * Time.fixedDeltaTime);

            // 최종 속도: 목표 지점까지 이동 + 플레이어 속도 반영
            selectedRigidbody.linearVelocity = desiredVelocity + playerVelocity * 0.5f;

            // 카메라의 현재 Y축 회전각 계산
            float currentCameraYRotation = cam.transform.eulerAngles.y;

            // 회전 처리
            Quaternion targetRotation = originalRotation;

            if (followCameraRotationY)
            {
                // 카메라 회전과 오브젝트 회전의 상대적 차이를 유지
                float yRotationDelta = Mathf.DeltaAngle(initialCameraYRotation, currentCameraYRotation);
                Vector3 originalEuler = originalRotation.eulerAngles;
                float targetYRotation = originalEuler.y + yRotationDelta;

                // X와 Z 회전은 그대로 유지하고 Y 회전만 업데이트
                targetRotation = Quaternion.Euler(originalEuler.x, targetYRotation, originalEuler.z);
            }

            selectedRigidbody.MoveRotation(targetRotation); // 회전 적용
        }
    }

    void HandleHighlighting(Ray ray)
    {
        // 레이가 선택 가능 오브젝트에 닿은 경우
        if (Physics.Raycast(ray, out RaycastHit hit, rayDistance))
        {
            GameObject hitObj = hit.collider.gameObject;

            if (((1 << hitObj.layer) & selectableLayer) != 0)
            {
                // 이전 하이라이트가 다른 오브젝트였다면 끄기
                if (lastOutline != null && lastOutline.gameObject != hitObj)
                {
                    lastOutline.enabled = false;
                    lastOutline = null;
                }

                // 새로운 오브젝트에 Outline 적용
                if (lastOutline == null)
                {
                    lastOutline = EnableOutline(hitObj, Color.yellow);
                }
                else
                {
                    // 이미 존재할 경우 색상만 노란색으로 설정
                    lastOutline.OutlineColor = Color.yellow;
                }
            }
            else
            {
                ClearHighlight();
            }
            if(((1 << hitObj.layer) & Wand) != 0)
            {
                Debug.Log("지팡이 입니다.");
            }

        }
        else
        {
            ClearHighlight();
        }
    }

    void HandleSelection(Ray ray)
    {
        // 좌클릭 시 선택 처리
        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(ray, out RaycastHit hit, rayDistance))
            {
                GameObject hitObj = hit.collider.gameObject;
                if (((1 << hitObj.layer) & selectableLayer) == 0) return;

                selectedObject = hitObj.transform;
                moveVelocity = Vector3.zero;
                currentRotationVelocity = 0f;

                // 히트 포인트와 오브젝트 중심 간의 오프셋 계산
                grabOffset = hit.point - selectedObject.position;

                // 카메라로부터의 거리 저장
                grabDistance = Vector3.Distance(cam.transform.position, hit.point);

                // 카메라 기준 오브젝트 위치의 로컬 좌표 계산
                Vector3 directionToHit = hit.point - cam.transform.position;
                Vector3 forwardDistance = Vector3.Project(directionToHit, cam.transform.forward);

                // 카메라 기준 로컬 좌표 (좌우, 상하) 구하기
                Vector3 rightComponent = Vector3.Project(directionToHit, cam.transform.right);
                Vector3 upComponent = Vector3.Project(directionToHit, cam.transform.up);

                float rightDistance = Vector3.Dot(rightComponent, cam.transform.right);
                float upDistance = Vector3.Dot(upComponent, cam.transform.up);

                grabbedLocalPosition = new Vector3(rightDistance, upDistance, 0);

                // 선택 시 오브젝트의 초기 회전값 저장
                originalRotation = selectedObject.rotation;

                // 카메라 현재 Y축 회전 저장
                initialCameraYRotation = cam.transform.eulerAngles.y;

                // 오브젝트와 카메라 간의 Y축 회전 차이 저장
                initialYRotationOffset = Mathf.DeltaAngle(initialCameraYRotation, originalRotation.eulerAngles.y);

                // Rigidbody 물리 설정 저장 및 비활성화
                selectedRigidbody = selectedObject.GetComponent<Rigidbody>();
                if (selectedRigidbody != null)
                {
                    wasKinematic = selectedRigidbody.isKinematic;
                    hadGravity = selectedRigidbody.useGravity;

                    selectedRigidbody.useGravity = false;
                    selectedRigidbody.isKinematic = false;

                    objectCollider = selectedRigidbody.GetComponent<Collider>();

                    if (playerCollider != null && objectCollider != null)
                    {
                        Physics.IgnoreCollision(playerCollider, objectCollider, true);
                    }
                }

                selectedRenderer = selectedObject.GetComponent<Renderer>();

                // ✅ Outline 설정 (항상 녹색으로 덮어쓰기)
                selectedOutline = EnableOutline(hitObj, Color.green);
                selectedOutline.OutlineColor = Color.green;
                selectedOutline.enabled = true;

                // 기존 감지용 outline 비활성화
                if (lastOutline != null && lastOutline != selectedOutline)
                {
                    lastOutline.enabled = false;
                    lastOutline = null;
                }
            }
        }
    }

    void ReleaseSelectedObject()
    {
        if (selectedObject == null) return;

        // Rigidbody 설정 복원
        if (selectedRigidbody != null)
        {
            selectedRigidbody.isKinematic = wasKinematic;
            selectedRigidbody.useGravity = hadGravity;
            selectedRigidbody.linearVelocity = Vector3.zero;

            if (playerCollider != null && objectCollider != null)
            {
                Physics.IgnoreCollision(playerCollider, objectCollider, false);
            }

            selectedRigidbody = null;
            objectCollider = null;
        }

        // 선택 Outline 비활성화
        if (selectedOutline != null)
        {
            selectedOutline.enabled = false;
            selectedOutline = null;
        }

        selectedRenderer = null;
        selectedObject = null;
        moveVelocity = Vector3.zero;
        grabOffset = Vector3.zero;
        grabbedLocalPosition = Vector3.zero;
    }

    // 오브젝트에 Outline 컴포넌트 추가하거나 가져오고 색상 설정
    Outline EnableOutline(GameObject obj, Color color)
    {
        Outline outline = obj.GetComponent<Outline>();
        if (outline == null)
        {
            outline = obj.AddComponent<Outline>();
            outline.OutlineMode = Outline.Mode.OutlineVisible;
            outline.OutlineWidth = 5f;
        }

        outline.OutlineColor = color;
        outline.enabled = true;
        return outline;
    }



    // 하이라이트 제거
    void ClearHighlight()
    {
        if (lastOutline != null)
        {
            lastOutline.enabled = false;
            lastOutline = null;
        }
    }

    //2024-04-18 추가
    //현재 선택된 오브젝트를 외부에서 확인
    public Transform GetSelectedObject()
    {
        return selectedObject;
    }

}