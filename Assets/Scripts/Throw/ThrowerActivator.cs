//using UnityEngine;

//public class ThrowActivator : MonoBehaviour, IInteractable
//{
//    // 현재 버튼이 눌려있는지 상태를 저장하는 프로퍼티
//    public bool IsPressed { get; private set; }

//    // 버튼 눌림 효과를 담당하는 컴포넌트 참조
//    private ThrowerButtonPushEffect pushEffect;

//    // 초기화: 컴포넌트 참조하고 이벤트 구독
//    private void Awake()
//    {
//        pushEffect = GetComponent<ThrowerButtonPushEffect>();

//        // pushEffect가 있으면 애니메이션 완료 이벤트 구독
//        if (pushEffect != null)
//        {
//            pushEffect.OnPushComplete += OnPushComplete;
//        }
//    }

//    // IInteractable 인터페이스의 인터랙션 진입점
//    public void Interact()
//    {
//        // 버튼 눌림 시도
//        PressButton();
//    }

//    // 버튼을 누르는 동작 수행
//    public void PressButton()
//    {
//        // 이미 눌려있으면 중복 실행 방지
//        if (!IsPressed)
//        {
//            IsPressed = true;

//            // 푸시 효과 시작 (애니메이션 재생)
//            pushEffect?.StartPushEffect();
//        }
//    }

//    // 푸시 애니메이션 완료 시 호출되는 콜백
//    private void OnPushComplete()
//    {
//        // 버튼 상태 해제 (다시 누를 수 있게)
//        ReleaseButton();
//    }

//    // 버튼 상태를 눌리지 않은 상태로 변경
//    public void ReleaseButton()
//    {
//        IsPressed = false;
//    }

//    // 오브젝트가 파괴될 때 이벤트 구독 해제 (메모리 누수 방지)
//    private void OnDestroy()
//    {
//        if (pushEffect != null)
//        {
//            pushEffect.OnPushComplete -= OnPushComplete;
//        }
//    }
//}

    using UnityEngine;

    public class ThrowActivator : MonoBehaviour, IInteractable, IButtonControllable
{
        public bool IsPressed { get; private set; }
        private ThrowerButtonPushEffect pushEffect;

    [Header("회전 애니메이션 설정")]
    [Tooltip("회전 애니메이션을 실행할 ThrowerAnimation 스크립트")]
    public ThrowerAnimation throwerAnimation;

    [Header("발사 관련 설정")]
        [Tooltip("발사 대상 오브젝트들이 모여있는 부모 트랜스폼")]
        public Transform launchPlatform;

        [Tooltip("발사할 힘 크기")]
        public float launchForce = 10f;
        [Tooltip("발사할 힘 크기")]
        public float changeableValue = 0.1f;

    [Tooltip("x축 발사 방향의 힘 배율")]
        public float xForceMultiplier = 1f;

        [Tooltip("y축 발사 방향의 힘 배율")]
        public float yForceMultiplier = 1f;

        private void Awake()
        {
            pushEffect = GetComponent<ThrowerButtonPushEffect>();
            if (pushEffect != null)
            {
                pushEffect.OnPushComplete += OnPushComplete;
            }
        }

        public void Interact()
        {
            PressButton();
        }

        public void PressButton()
        {
            if (!IsPressed)
            {
                IsPressed = true;
                pushEffect?.StartPushEffect();

                
                LaunchObjects();
                throwerAnimation?.RotateAroundParentPivot();



        }   
        }
    //private void LaunchObjects()
    //{
    //    if (launchPlatform == null)
    //    {
    //        Debug.LogWarning("발사대(launchPlatform) 설정이 필요합니다.");
    //        return;
    //    }

    //    Vector3 center = launchPlatform.position;
    //    Vector3 halfExtents = new Vector3(1f, 1f, 1f);  // 필요에 따라 조절

    //    Collider[] colliders = Physics.OverlapBox(center, halfExtents);

    //    foreach (Collider col in colliders)
    //    {
    //        Rigidbody rb = col.attachedRigidbody;
    //        if (rb != null)
    //        {
    //            Debug.Log($"발사 대상: {rb.gameObject.name}");

    //            Vector3 force = new Vector3(xForceMultiplier, yForceMultiplier, 0f).normalized * launchForce;

    //            // 질량 무시하고 속도 즉시 변경
    //            rb.AddForce(force, ForceMode.VelocityChange);
    //        }
    //    }
    //}
    //private void LaunchObjects()
    //{
    //    if (launchPlatform == null)
    //    {
    //        Debug.LogWarning("발사대(launchPlatform) 설정이 필요합니다.");
    //        return;
    //    }

    //    Vector3 center = launchPlatform.position;
    //    Vector3 halfExtents = new Vector3(1f, 1f, 1f);  // 필요에 따라 조절

    //    Collider[] colliders = Physics.OverlapBox(center, halfExtents);

    //    foreach (Collider col in colliders)
    //    {
    //        Rigidbody rb = col.attachedRigidbody;
    //        if (rb != null)
    //        {
    //            Debug.Log($"발사 대상: {rb.gameObject.name}");

    //            // 발사판의 x, y축 방향에 따라 힘 계산
    //            Vector3 forceDirection = (launchPlatform.right * xForceMultiplier + launchPlatform.up * yForceMultiplier).normalized;
    //            Vector3 force = forceDirection * launchForce;

    //            rb.AddForce(force, ForceMode.VelocityChange);
    //        }
    //    }
    //}
    private void LaunchObjects()
    {
        if (launchPlatform == null)
        {
            Debug.LogWarning("발사대(launchPlatform) 설정이 필요합니다.");
            return;
        }

        // 자식에서 BoxCollider 찾기 (첫 번째 자식에 있다고 가정)
        BoxCollider boxCollider = launchPlatform.GetComponentInChildren<BoxCollider>();
        if (boxCollider == null)
        {
            Debug.LogWarning("launchPlatform 자식에 BoxCollider가 필요합니다.");
            return;
        }

        Vector3 center = boxCollider.bounds.center; // 월드 공간 중심
        Vector3 halfExtents = boxCollider.bounds.extents; // 월드 공간 반 크기
        Quaternion rotation = boxCollider.transform.rotation; // 콜라이더가 붙은 자식의 회전

        Debug.Log($"OverlapBox center: {center}, halfExtents: {halfExtents}, rotation: {rotation.eulerAngles}");

        Collider[] colliders = Physics.OverlapBox(center, halfExtents, rotation);

        foreach (Collider col in colliders)
        {
            Rigidbody rb = col.attachedRigidbody;
            if (rb != null)
            {
                Debug.Log($"발사 대상: {rb.gameObject.name}");

                Vector3 forceDirection = (launchPlatform.right * xForceMultiplier + launchPlatform.up * yForceMultiplier).normalized;
                Vector3 force = forceDirection * launchForce;

                rb.AddForce(force, ForceMode.VelocityChange);
            }
        }

    }









    private void OnPushComplete()
        {
            ReleaseButton();
        }

        public void ReleaseButton()
        {
            IsPressed = false;
        }

        private void OnDestroy()
        {
            if (pushEffect != null)
            {
                pushEffect.OnPushComplete -= OnPushComplete;
            }
        }

    public void Increase()
    {
        if (launchForce <= 40)
        {
            launchForce += changeableValue;
        }
        //Debug.Log($"{gameObject.name} 숫자 증가: {currentValue}");
    }

    public void Decrease()
    {
        if (launchForce >= 10)
        {
            launchForce -= changeableValue;
        }
        //Debug.Log($"{gameObject.name} 숫자 감소: {currentValue}");
    }




}
