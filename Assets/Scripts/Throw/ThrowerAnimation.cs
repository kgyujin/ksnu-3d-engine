using UnityEngine;
using System.Collections;

public class ThrowerAnimation : MonoBehaviour
{
    public Transform childObject;         // 회전시킬 자식
    public Transform pivotTransform;      // 피벗이 될 부모
    public float rotationAngle = 45f;     // 회전 각도
    public float rotationDuration = 0.5f; // 회전 시간
    public bool rotateClockwise = true;   // true면 시계방향, false면 반시계

    private Vector3 originalLocalPosition;
    private Quaternion originalLocalRotation;

    void Start()
    {
        if (childObject == null)
            childObject = transform;

        if (pivotTransform == null)
            pivotTransform = transform.parent;

        originalLocalPosition = childObject.localPosition;
        originalLocalRotation = childObject.localRotation;
    }

    public void RotateAroundParentPivot()
    {
        StopAllCoroutines();

        float direction = rotateClockwise ? -1f : 1f; // 시계방향이면 음수
        StartCoroutine(RotateAround(pivotTransform.position, Vector3.forward, direction * rotationAngle, true));
    }

    IEnumerator RotateAround(Vector3 pivot, Vector3 axis, float angle, bool returnBack)
    {
        float elapsed = 0f;
        float currentAngle = 0f;
        float sign = Mathf.Sign(angle);
        float absAngle = Mathf.Abs(angle);

        while (elapsed < rotationDuration)
        {
            float step = (Time.deltaTime / rotationDuration) * absAngle;
            childObject.RotateAround(pivot, axis, sign * step);
            currentAngle += step;
            elapsed += Time.deltaTime;
            yield return null;
        }

        float correction = absAngle - currentAngle;
        childObject.RotateAround(pivot, axis, sign * correction);

        // 복귀 (반대 방향으로 회전)
        if (returnBack)
        {
            elapsed = 0f;
            currentAngle = 0f;

            while (elapsed < rotationDuration)
            {
                float step = (Time.deltaTime / rotationDuration) * absAngle;
                childObject.RotateAround(pivot, axis, -sign * step);
                currentAngle += step;
                elapsed += Time.deltaTime;
                yield return null;
            }

            correction = absAngle - currentAngle;
            childObject.RotateAround(pivot, axis, -sign * correction);

            // 위치, 회전 복구
            childObject.localPosition = originalLocalPosition;
            childObject.localRotation = originalLocalRotation;
        }
    }

    //void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.Space))
    //    {
    //        RotateAroundParentPivot();
    //    }
    //}
}
