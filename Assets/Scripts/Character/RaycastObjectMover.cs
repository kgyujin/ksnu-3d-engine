using UnityEngine;

public class RaycastObjectMover : MonoBehaviour
{
    [Header("감지 설정")]
    public Transform wandPoint;  // 플레이어 옆에 지팡이를 붙일 위치
    private GameObject selectedWand;  // 현재 선택된 지팡이
    private Vector3 selectedWandOriginalPosition;  // 지팡이 원래 위치 저장용
    private Quaternion selectedWandOriginalRotation;  // 지팡이 원래 회전 저장용

    public LayerMask selectableLayer;  // 일반 선택 가능한 오브젝트 Layer
    public LayerMask Wand;  // 지팡이 Layer
    public float rayDistance = 5f;  // Raycast 거리
    public float moveForce = 500f;  // 물체를 끌어올 때 힘
    public bool drawDebugRay = true;  // 디버그용 Ray 표시 여부
    public LayerMask interactableLayer;  // 인터랙트 가능한 오브젝트 Layer
    private Outline lastInteractOutline = null;  // 마지막으로 강조된 인터랙트 오브젝트

    [Header("회전 설정")]
    public float rotationSmoothTime = 0.1f;  // 회전 부드럽게 만드는 시간
    public bool followCameraRotationY = true;  // 카메라 회전을 따라갈지 여부

    private Transform selectedObject = null;  // 현재 잡은 오브젝트
    private Vector3 moveVelocity = Vector3.zero;  // 이동 속도
    private float currentRotationVelocity;

    private Outline lastOutline = null;  // 마지막으로 강조된 일반 오브젝트
    private Outline selectedOutline = null;  // 현재 잡힌 오브젝트의 강조선

    private Renderer selectedRenderer = null;  // 현재 잡힌 오브젝트 렌더러
    private Rigidbody selectedRigidbody = null;  // 현재 잡힌 오브젝트 Rigidbody

    private Camera cam;
    private Rigidbody playerRigidbody = null;  // 플레이어 Rigidbody
    private Collider playerCollider = null;  // 플레이어 Collider
    private Collider objectCollider = null;  // 잡힌 오브젝트의 Collider

    private bool wasKinematic = false;  // 기존에 Kinematic 상태였는지 저장
    private bool hadGravity = false;  // 기존에 Gravity가 있었는지 저장

    private Quaternion originalRotation;
    private float initialYRotationOffset;
    private float initialCameraYRotation;
    private Vector3 grabOffset = Vector3.zero;
    private float grabDistance = 0f;
    private Vector3 grabbedLocalPosition;

    // 지팡이가 선택됐는지 여부를 확인하기 위한 속성 추가
    public bool IsWandSelected { get { return selectedWand != null; } }
    public Transform WandTip { get { return selectedWand != null ? selectedWand.transform : null; } }

    void Start()
    {
        cam = Camera.main;

        // 플레이어 오브젝트 가져오기
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            playerCollider = player.GetComponent<Collider>();
            playerRigidbody = player.GetComponent<Rigidbody>();
        }
    }



    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, GetRayDistance(), ~0, QueryTriggerInteraction.Ignore))

            {
                GameObject hitObj = hit.collider.gameObject;

                if (((1 << hitObj.layer) & Wand) != 0)
                {
                    ItemSelectManager itemSelecteManager = GetComponent<ItemSelectManager>();
                    if (itemSelecteManager != null)
                    {
                        itemSelecteManager.WearItem(hitObj);
                        selectedWand = hitObj;
                    }
                    return;
                }
            }
        }

        if (cam == null) return;

        Ray centerRay = cam.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        if (drawDebugRay)
            Debug.DrawRay(centerRay.origin, centerRay.direction * GetRayDistance(), Color.cyan);

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (selectedObject == null && selectedWand == null)
        {
            rayDistance += scroll * 5f;
            rayDistance = Mathf.Clamp(rayDistance, 1f, 100f);
        }
        else if (selectedObject != null && Mathf.Abs(scroll) > 0.01f)
        {
            grabDistance -= scroll * 5f;
            grabDistance = Mathf.Clamp(grabDistance, 1f, 100f);
        }

        // 마우스 휠로 거리 조절
        //float scroll = Input.GetAxis("Mouse ScrollWheel");
        //if (selectedObject == null)
        //{
        //    rayDistance += scroll * 5f;
        //    rayDistance = Mathf.Clamp(rayDistance, 1f, 100f);
        //}
        //else if (Mathf.Abs(scroll) > 0.01f)
        //{
        //    grabDistance -= scroll * 5f;
        //    grabDistance = Mathf.Clamp(grabDistance, 1f, 100f);
        //}

        if (selectedObject == null)
        {
            HandleHighlighting(centerRay);
            HandleSelection(centerRay);
        }

        if (Input.GetMouseButtonDown(1))
        {
            ReleaseSelectedObject();
        }

        HandleInteraction(centerRay);
    }


    void FixedUpdate()
    {
        // 오브젝트를 잡고 있으면 물리적으로 이동 처리
        if (selectedObject != null && selectedRigidbody != null)
        {
            Vector3 targetPos = cam.transform.position + cam.transform.forward * grabDistance +
                                cam.transform.right * grabbedLocalPosition.x +
                                cam.transform.up * grabbedLocalPosition.y - grabOffset;

            Vector3 direction = targetPos - selectedRigidbody.position;

            Vector3 playerVelocity = playerRigidbody != null ? playerRigidbody.linearVelocity : Vector3.zero;
            Vector3 desiredVelocity = direction.normalized * Mathf.Min(direction.magnitude * 10f, moveForce * Time.fixedDeltaTime);

            selectedRigidbody.linearVelocity = desiredVelocity + playerVelocity * 0.5f;

            // 카메라 회전에 따라 회전 처리
            float currentCameraYRotation = cam.transform.eulerAngles.y;
            Quaternion targetRotation = originalRotation;

            if (followCameraRotationY)
            {
                float yRotationDelta = Mathf.DeltaAngle(initialCameraYRotation, currentCameraYRotation);
                Vector3 originalEuler = originalRotation.eulerAngles;
                float targetYRotation = originalEuler.y + yRotationDelta;
                targetRotation = Quaternion.Euler(originalEuler.x, targetYRotation, originalEuler.z);
            }

            selectedRigidbody.MoveRotation(targetRotation);
        }
    }

    // 하이라이트 처리 (Outline)
    void HandleHighlighting(Ray ray)
    {
        bool hitSelectable = false;
        bool hitInteractable = false;

        if (Physics.Raycast(ray, out RaycastHit hit, GetRayDistance(), ~0, QueryTriggerInteraction.Ignore))
        {
            GameObject hitObj = hit.collider.gameObject;

            // 선택 가능한 오브젝트 하이라이트
            if (((1 << hitObj.layer) & selectableLayer) != 0)
            {
                hitSelectable = true;
                lastOutline = UpdateOutline(hitObj, lastOutline, Color.yellow);
            }
            else if (lastOutline != null)
            {
                lastOutline.enabled = false;
                lastOutline = null;
            }

            // 인터랙트 가능한 오브젝트 하이라이트
            if (((1 << hitObj.layer) & interactableLayer) != 0)
            {
                hitInteractable = true;

                IInteractable interactable = hitObj.GetComponent<IInteractable>();
                if (!(interactable is ButtonActivator button && button.IsPressed))
                {
                    lastInteractOutline = UpdateOutline(hitObj, lastInteractOutline, Color.yellow);
                }
            }
            else if (lastInteractOutline != null)
            {
                lastInteractOutline.enabled = false;
                lastInteractOutline = null;
            }

            // Wand 처리
            if (((1 << hitObj.layer) & Wand) != 0)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    ItemSelectManager itemSelecteManager = GetComponent<ItemSelectManager>();
                    if (itemSelecteManager != null)
                    {
                        itemSelecteManager.WearItem(hitObj);
                        //selectedWand = hitObj; // 지팡이 선택 상태 업데이트
                    }
                }
            }
        }
        else
        {
            // 아무것도 감지되지 않았을 때 모든 하이라이트 제거
            ClearHighlight();
        }

        if (!hitSelectable && !hitInteractable)
            ClearHighlight();
    }

    // Outline 업데이트
    Outline UpdateOutline(GameObject obj, Outline current, Color color)
    {
        if (current != null && current.gameObject != obj)
        {
            current.enabled = false;
            current = null;
        }

        if (current == null)
            current = EnableOutline(obj, color);
        else
            current.OutlineColor = color;

        return current;
    }

    // 오브젝트 선택 처리
    void HandleSelection(Ray ray)
    {
        // 지팡이가 선택되지 않은 상태에서는 선택 불가능하게 수정
        if (!IsWandSelected) return;

        if (Input.GetMouseButtonDown(0) && Physics.Raycast(ray, out RaycastHit hit, GetRayDistance(), ~0, QueryTriggerInteraction.Ignore))
        {
            GameObject hitObj = hit.collider.gameObject;
            if (((1 << hitObj.layer) & selectableLayer) == 0) return;

            selectedObject = hitObj.transform;
            moveVelocity = Vector3.zero;
            currentRotationVelocity = 0f;
            grabOffset = hit.point - selectedObject.position;
            grabDistance = Vector3.Distance(cam.transform.position, hit.point);

            Vector3 directionToHit = hit.point - cam.transform.position;
            grabbedLocalPosition = new Vector3(
                Vector3.Dot(directionToHit, cam.transform.right),
                Vector3.Dot(directionToHit, cam.transform.up),
                0);

            originalRotation = selectedObject.rotation;
            initialCameraYRotation = cam.transform.eulerAngles.y;
            initialYRotationOffset = Mathf.DeltaAngle(initialCameraYRotation, originalRotation.eulerAngles.y);

            selectedRigidbody = selectedObject.GetComponent<Rigidbody>();
            if (selectedRigidbody != null)
            {
                wasKinematic = selectedRigidbody.isKinematic;
                hadGravity = selectedRigidbody.useGravity;

                selectedRigidbody.useGravity = false;
                selectedRigidbody.isKinematic = false;

                objectCollider = selectedRigidbody.GetComponent<Collider>();
                if (playerCollider != null && objectCollider != null)
                    Physics.IgnoreCollision(playerCollider, objectCollider, true);
            }

            selectedRenderer = selectedObject.GetComponent<Renderer>();
            selectedOutline = EnableOutline(hitObj, Color.green);

            if (lastOutline != null && lastOutline != selectedOutline)
            {
                lastOutline.enabled = false;
                lastOutline = null;
            }
        }
    }

    // 오브젝트 해제 처리
    public void ReleaseSelectedObject()
    {
        if (selectedObject == null) return;

        if (selectedRigidbody != null)
        {
            selectedRigidbody.isKinematic = wasKinematic;
            selectedRigidbody.useGravity = hadGravity;
            selectedRigidbody.linearVelocity = Vector3.zero;

            if (playerCollider != null && objectCollider != null)
                Physics.IgnoreCollision(playerCollider, objectCollider, false);

            selectedRigidbody = null;
            objectCollider = null;
        }

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

    // Outline 생성
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

    void ClearHighlight()
    {
        if (lastOutline != null)
        {
            lastOutline.enabled = false;
            lastOutline = null;
        }
        if (lastInteractOutline != null)
        {
            lastInteractOutline.enabled = false;
            lastInteractOutline = null;
        }
    }

    public Transform GetSelectedObject() => selectedObject;

    // 인터랙션 처리 (버튼 등)
    void HandleInteraction(Ray ray)
    {
        if (Input.GetMouseButtonDown(0) && Physics.Raycast(ray, out RaycastHit hit, GetRayDistance(), ~0, QueryTriggerInteraction.Ignore))
        {
            GameObject hitObj = hit.collider.gameObject;
            if (((1 << hitObj.layer) & interactableLayer) != 0)
            {
                IInteractable interactable = hitObj.GetComponent<IInteractable>();
                interactable?.Interact();
            }
        }
    }

    // 숫자키로 지팡이를 장착할 때도 selectedWand가 정상적으로 설정되어 GetRayDistance()가 작동
    public void SetSelectedWand(GameObject wand)
    {
        selectedWand = wand;
    }

    // 외부에서 현재 잡고 있는 오브젝트 강제 해제(RespawnTrigger)
    public void ForceReleaseSelectedObject()
    {
        ReleaseSelectedObject();
    }

    // 지팡이 해제 처리
    public void UnselectWand()
    {
        selectedWand = null;
    }

    // 선택된 지팡이의 raycast 거리 가져오기
    private float GetRayDistance()
    {
        if (selectedWand != null)
        {
            WandItem wandItem = selectedWand.GetComponent<WandItem>();
            if (wandItem != null)
            {
                return wandItem.raycast_distance;
            }
        }

        return rayDistance; // 기본값
    }

    public void SetRaycastDistance(float distance)
    {
        rayDistance = distance;
        //Debug.Log("Raycast 거리 설정됨: " + raycastDistance);
    }
}