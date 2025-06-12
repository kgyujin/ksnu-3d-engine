using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class WandLineDrawer : MonoBehaviour
{
    public RaycastObjectMover objectMover;
    public Transform wandTip; // 활성화된 지팡이의 끝

    private LineRenderer lineRenderer;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = 0.03f;
        lineRenderer.endWidth = 0.01f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = Color.cyan;
        lineRenderer.endColor = Color.blue;
        lineRenderer.enabled = false;

        if (objectMover == null)
            objectMover = FindObjectOfType<RaycastObjectMover>();
    }

    void Update()
    {
        if (objectMover == null) return;

        // 이 지팡이가 현재 선택된 지팡이가 아니라면 라인 끄기
        if (objectMover.WandTip == null || objectMover.WandTip.gameObject != this.gameObject)
        {
            lineRenderer.enabled = false;
            return;
        }

        // 지팡이 끝점 초기화
        if (wandTip == null)
        {
            Transform tipTransform = objectMover.WandTip.Find("Tip");
            wandTip = tipTransform != null ? tipTransform : objectMover.WandTip;
        }

        Transform target = objectMover.GetSelectedObject();

        // 타겟 있을 때만 라인 표시
        if (wandTip != null && target != null)
        {
            lineRenderer.enabled = true;
            lineRenderer.SetPosition(0, wandTip.position);
            lineRenderer.SetPosition(1, target.position);
        }
        else
        {
            lineRenderer.enabled = false;
        }
    }
}