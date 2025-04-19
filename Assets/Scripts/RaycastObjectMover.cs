using UnityEngine;

public class RaycastObjectMover : MonoBehaviour
{
    [Header("감지 설정")]
    public LayerMask selectableLayer; // 선택 가능한 오브젝트 레이어
    public float rayDistance = 5f; // 레이 길이
    public float smoothTime = 0.1f; // 이동 부드러움 정도
    public bool drawDebugRay = true; // 디버그용 레이 표시 여부

    private Transform selectedObject = null; // 선택된 오브젝트
    private Vector3 moveVelocity = Vector3.zero; // 스무스 이동에 사용될 속도 벡터

    private Outline lastOutline = null; // 하이라이트(감지)된 오브젝트의 Outline
    private Outline selectedOutline = null; // 선택(잡기)된 오브젝트의 Outline

    private Renderer selectedRenderer = null; // 선택된 오브젝트의 렌더러
    private Rigidbody selectedRigidbody = null; // 선택된 오브젝트의 리지드바디

    private bool wasKinematic = false; // 원래 키네마틱 여부 저장
    private bool hadGravity = false; // 원래 중력 여부 저장

    private Camera cam; // 카메라

    void Start()
    {
        cam = Camera.main; // 메인 카메라 참조
    }

    void Update()
    {
        if (cam == null) return;

        // 카메라 중앙 기준으로 레이 쏘기
        Ray ray = cam.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));

        if (drawDebugRay)
            Debug.DrawRay(ray.origin, ray.direction * rayDistance, Color.cyan);

        // 마우스 휠로 레이 거리 조절
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        rayDistance += scroll * 5f;
        rayDistance = Mathf.Clamp(rayDistance, 1f, 100f);

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
    }

    void FixedUpdate()
    {
        // 오브젝트가 선택된 경우 카메라 앞쪽으로 이동
        if (selectedObject != null)
        {
            Vector3 targetPos = cam.transform.position + cam.transform.forward * rayDistance;

            if (selectedRigidbody != null)
            {
                selectedRigidbody.MovePosition(targetPos); // Rigidbody가 있으면 MovePosition 사용
            }
            else
            {
                selectedObject.position = Vector3.SmoothDamp(
                    selectedObject.position,
                    targetPos,
                    ref moveVelocity,
                    smoothTime,
                    Mathf.Infinity,
                    Time.fixedDeltaTime
                );
            }
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

                // Rigidbody 물리 설정 저장 및 비활성화
                selectedRigidbody = selectedObject.GetComponent<Rigidbody>();
                if (selectedRigidbody != null)
                {
                    wasKinematic = selectedRigidbody.isKinematic;
                    hadGravity = selectedRigidbody.useGravity;

                    selectedRigidbody.useGravity = false;
                    selectedRigidbody.isKinematic = true;
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
            selectedRigidbody = null;
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
