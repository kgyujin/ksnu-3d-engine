using UnityEngine;

public class ObjectSelector_HoldPoint : MonoBehaviour
{
    [Header("감지 설정")]
    public LayerMask selectableLayer;
    public float followSpeed = 10f;
    public float holdYOffset = 1.5f;
    public float moveSpeed = 2.0f;
    public bool drawDebugRay = true;

    [Header("오브젝트 이동 타겟")]
    public Transform holdAnchor;

    private Camera mainCam;
    private GameObject hoveredObject;
    private GameObject heldObject;
    private Outline lastOutline;

    private Collider playerCollider;
    private Collider heldCollider;
    private Rigidbody heldRigidbody;

    private Vector3 manualOffset = Vector3.zero;

    private float originalMass;
    private float originalDrag;
    private float originalAngularDrag;

    void Start()
    {
        mainCam = Camera.main;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            playerCollider = player.GetComponent<Collider>();
    }

    void Update()
    {
        Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (drawDebugRay)
            Debug.DrawRay(ray.origin, ray.direction * 100f, Color.cyan);

        // 잡은 오브젝트 이동 처리
        if (heldObject != null)
        {
            // 카메라 기준 입력 방향 계산
            Vector3 input = Vector3.zero;

            if (Input.GetKey(KeyCode.W)) input += mainCam.transform.forward;
            if (Input.GetKey(KeyCode.S)) input -= mainCam.transform.forward;
            if (Input.GetKey(KeyCode.A)) input -= mainCam.transform.right;
            if (Input.GetKey(KeyCode.D)) input += mainCam.transform.right;
            if (Input.GetKey(KeyCode.E)) input += Vector3.up;
            if (Input.GetKey(KeyCode.Q)) input -= Vector3.up;

            manualOffset += input.normalized * moveSpeed * Time.deltaTime;

            // 최종 목표 위치
            Vector3 basePos = holdAnchor.position + Vector3.up * holdYOffset;
            Vector3 targetPos = basePos + manualOffset;

            if (heldRigidbody != null)
            {
                heldRigidbody.MovePosition(Vector3.Lerp(heldObject.transform.position, targetPos, Time.deltaTime * followSpeed));
                heldRigidbody.MoveRotation(Quaternion.identity);

                heldRigidbody.linearVelocity = Vector3.zero;
                heldRigidbody.angularVelocity = Vector3.zero;
            }
            else
            {
                heldObject.transform.position = Vector3.Lerp(heldObject.transform.position, targetPos, Time.deltaTime * followSpeed);
                heldObject.transform.rotation = Quaternion.identity;
            }

            // 놓기 (우클릭)
            if (Input.GetMouseButtonDown(1))
            {
                if (heldRigidbody != null)
                {
                    heldRigidbody.useGravity = true;
                    heldRigidbody.mass = originalMass;
                    heldRigidbody.linearDamping = originalDrag;
                    heldRigidbody.angularDamping = originalAngularDrag;
                }

                ClearHighlight();
                heldObject = null;
                heldCollider = null;
                heldRigidbody = null;
                manualOffset = Vector3.zero;
            }

            return;
        }

        // 잡고 있지 않을 때 감지
        if (Physics.Raycast(ray, out hit, 100f))
        {
            GameObject hitObj = hit.collider.gameObject;

            if (((1 << hitObj.layer) & selectableLayer) != 0)
            {
                if (hoveredObject != hitObj)
                {
                    ClearHighlight();
                    hoveredObject = hitObj;
                    lastOutline = EnableOutline(hoveredObject, Color.yellow);
                }

                // 클릭해서 잡기
                if (Input.GetMouseButtonDown(0))
                {
                    heldObject = hoveredObject;
                    EnableOutline(heldObject, Color.green);

                    heldCollider = heldObject.GetComponent<Collider>();
                    heldRigidbody = heldObject.GetComponent<Rigidbody>();

                    if (heldRigidbody != null)
                    {
                        originalMass = heldRigidbody.mass;
                        originalDrag = heldRigidbody.linearDamping;
                        originalAngularDrag = heldRigidbody.angularDamping;

                        heldRigidbody.useGravity = false;
                        heldRigidbody.linearVelocity = Vector3.zero;
                        heldRigidbody.angularVelocity = Vector3.zero;

                        heldRigidbody.mass = 0.1f;
                        heldRigidbody.linearDamping = 10f;
                        heldRigidbody.angularDamping = 10f;
                    }

                    manualOffset = Vector3.zero; // 이동 오프셋 초기화
                }
            }
            else
            {
                ClearHighlight();
                hoveredObject = null;
            }
        }
        else
        {
            ClearHighlight();
            hoveredObject = null;
        }
    }

    private Outline EnableOutline(GameObject obj, Color color)
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

    private void ClearHighlight()
    {
        if (lastOutline != null && heldObject != lastOutline.gameObject)
        {
            lastOutline.enabled = false;
            lastOutline = null;
        }
    }
}
