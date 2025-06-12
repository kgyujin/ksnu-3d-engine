////using UnityEngine;

////public class ValueChangeButton : MonoBehaviour
////{
////    public ThrowerButtonPushEffect pushEffect;
////    public GameObject targetObject; // Increase/Decrease 할 대상 오브젝트
////    public enum ChangeType { Increase, Decrease }
////    public ChangeType changeType;

////    private IButtonControllable controllable;

////    private void Awake()
////    {
////        if (pushEffect != null)
////            pushEffect.OnPushComplete += OnPushComplete;

////        if (targetObject != null)
////            controllable = targetObject.GetComponent<IButtonControllable>();
////    }

////    private void OnDestroy()
////    {
////        if (pushEffect != null)
////            pushEffect.OnPushComplete -= OnPushComplete;
////    }

////    private void OnPushComplete()
////    {
////        if (controllable == null) return;

////        switch (changeType)
////        {
////            case ChangeType.Increase:
////                controllable.Increase();
////                break;
////            case ChangeType.Decrease:
////                controllable.Decrease();
////                break;
////        }
////    }
////}


//using UnityEngine;

//public class ValueChangeButton : MonoBehaviour
//{
//    public ThrowerButtonPushEffect pushEffect;
//    public GameObject targetObject; // Increase/Decrease 할 대상 오브젝트
//    public enum ChangeType { Increase, Decrease }
//    public ChangeType changeType;

//    private IButtonControllable controllable;

//    // 버튼이 눌렸는지 확인하는 변수
//    private bool isPressed;

//    private void Awake()
//    {
//        if (pushEffect != null)
//            pushEffect.OnPushComplete += OnPushComplete;

//        if (targetObject != null)
//            controllable = targetObject.GetComponent<IButtonControllable>();
//    }

//    private void OnDestroy()
//    {
//        if (pushEffect != null)
//            pushEffect.OnPushComplete -= OnPushComplete;
//    }

//    private void Update()
//    {
//        // 마우스 좌클릭이 눌리면
//        if (Input.GetMouseButtonDown(0)) // 0은 좌클릭을 의미
//        {
//            // 버튼에 마우스가 눌린 상태일 때만 효과 실행
//            if (!isPressed)
//            {
//                pushEffect.StartPushEffect();
//            }
//        }
//    }

//    private void OnPushComplete()
//    {
//        // 디버그 로그 추가: OnPushComplete가 호출되었는지 확인
//        Debug.Log("Button Push Complete!");

//        // 중복 실행 방지: 버튼이 이미 눌렸으면 아무것도 하지 않음
//        if (isPressed || controllable == null) return;

//        // 버튼 눌림 상태 처리
//        isPressed = true;

//        // 버튼 상태에 따라 값을 증가 또는 감소
//        switch (changeType)
//        {
//            case ChangeType.Increase:
//                controllable.Increase();
//                break;
//            case ChangeType.Decrease:
//                controllable.Decrease();
//                break;
//        }

//        // 애니메이션이 끝나면 버튼 상태를 해제
//        ReleaseButton();
//    }

//    // 버튼을 해제하는 메소드
//    private void ReleaseButton()
//    {
//        // 버튼 상태를 초기화
//        Debug.Log("Button Released!");
//        isPressed = false;
//    }
//}


//using UnityEngine;

//public class ValueChangeButton : MonoBehaviour, IInteractable/*, IButtonControllable*/
//{
//    public bool IsPressed { get; private set; }
//    private ThrowerButtonPushEffect pushEffect;

//    private void Awake()
//    {
//        // 버튼 눌림 효과 관련 컴포넌트 초기화
//        pushEffect = GetComponent<ThrowerButtonPushEffect>();
//        if (pushEffect != null)
//        {
//            pushEffect.OnPushComplete += OnPushComplete; // 눌림 완료 후 호출될 이벤트 등록
//        }
//    }

//    // 인터랙션 메서드 - 버튼을 누름
//    public void Interact()
//    {
//        PressButton();
//    }

//    // 버튼을 눌렀을 때 실행되는 메서드
//    public void PressButton()
//    {
//        if (!IsPressed)
//        {
//            IsPressed = true;
//            pushEffect?.StartPushEffect(); // 버튼 눌림 애니메이션 효과 시작
//        }
//    }

//    // 버튼 눌림 효과가 끝났을 때 호출되는 메서드
//    private void OnPushComplete()
//    {
//        ReleaseButton(); // 버튼 놓음
//    }

//    // 버튼을 놓는 메서드
//    public void ReleaseButton()
//    {
//        IsPressed = false;
//    }

//    // 버튼 눌림 효과 완료 후 이벤트 해제 (파괴될 때)
//    private void OnDestroy()
//    {
//        if (pushEffect != null)
//        {
//            pushEffect.OnPushComplete -= OnPushComplete; // 이벤트 해제
//        }
//    }
//}


//using UnityEngine;
//using System.Collections;

//public class ValueChangeButton : MonoBehaviour, IInteractable
//{
//    public bool IsPressed { get; private set; }
//    private ThrowerButtonPushEffect pushEffect;
//    private IButtonControllable targetObject;  // 조작할 대상 객체
//    private Coroutine changeValueCoroutine;     // 값 변경을 위한 코루틴

//    private bool isIncreasing = true;  // 값이 증가할지 감소할지 여부

//    private void Awake()
//    {
//        pushEffect = GetComponent<ThrowerButtonPushEffect>();
//        if (pushEffect != null)
//        {
//            pushEffect.OnPushComplete += OnPushComplete;
//        }
//    }

//    // 인터랙션 메서드 - 버튼을 누름
//    public void Interact()
//    {
//        PressButton();
//    }

//    // 버튼을 눌렀을 때 실행되는 메서드
//    public void PressButton()
//    {
//        if (!IsPressed)
//        {
//            IsPressed = true;
//            pushEffect?.StartPushEffect();

//            // 타겟 객체가 설정되어 있으면 값을 조절
//            if (targetObject != null)
//            {
//                if (changeValueCoroutine != null)
//                    StopCoroutine(changeValueCoroutine);

//                changeValueCoroutine = StartCoroutine(ChangeValueOverTime());
//            }
//        }
//    }

//    private IEnumerator ChangeValueOverTime()
//    {
//        float interval = 1f; // 1초 간격으로 값을 변경
//        while (IsPressed)
//        {
//            if (isIncreasing)
//                targetObject.Increase();
//            else
//                targetObject.Decrease();

//            yield return new WaitForSeconds(interval);
//        }
//    }

//    // 버튼 눌림 효과가 끝났을 때 호출되는 메서드
//    private void OnPushComplete()
//    {
//        ReleaseButton(); // 버튼 놓음
//    }

//    // 버튼을 놓는 메서드
//    public void ReleaseButton()
//    {
//        IsPressed = false;

//        if (changeValueCoroutine != null)
//        {
//            StopCoroutine(changeValueCoroutine); // 코루틴 정지
//        }
//    }

//    // 타겟 객체 설정
//    public void SetTargetObject(IButtonControllable obj)
//    {
//        targetObject = obj;
//    }

//    // 값 변경 방향을 전환하는 메서드
//    public void ToggleIncreaseDecrease()
//    {
//        isIncreasing = !isIncreasing;
//    }

//    private void OnDestroy()
//    {
//        if (pushEffect != null)
//        {
//            pushEffect.OnPushComplete -= OnPushComplete; // 이벤트 해제
//        }
//    }
//}


//using UnityEngine;
//using System.Collections;

//public class ValueChangeButton : MonoBehaviour, IInteractable
//{
//    public bool IsPressed { get; private set; }
//    private ThrowerButtonPushEffect pushEffect;

//    // IButtonControllable을 구현한 MonoBehaviour 타입으로 변경
//    [SerializeField] private MonoBehaviour targetObject;  // 조작할 대상 객체, IButtonControllable을 구현한 컴포넌트
//    private IButtonControllable buttonControllableTarget;

//    private Coroutine changeValueCoroutine;     // 값 변경을 위한 코루틴

//    // 방향을 선택할 수 있는 enum 추가
//    public enum ValueChangeDirection
//    {
//        Increase,
//        Decrease
//    }

//    [SerializeField] private ValueChangeDirection changeDirection = ValueChangeDirection.Increase; // 증가/감소 방향 선택

//    private void Awake()
//    {
//        // targetObject가 MonoBehaviour 타입이므로, 이를 IButtonControllable로 캐스팅
//        buttonControllableTarget = targetObject as IButtonControllable;

//        if (buttonControllableTarget == null)
//        {
//            Debug.LogError("targetObject는 IButtonControllable을 구현한 컴포넌트여야 합니다.");
//        }

//        pushEffect = GetComponent<ThrowerButtonPushEffect>();
//        if (pushEffect != null)
//        {
//            pushEffect.OnPushComplete += OnPushComplete;
//        }
//    }

//    // 인터랙션 메서드 - 버튼을 누름
//    public void Interact()
//    {
//        PressButton();
//    }

//    // 버튼을 눌렀을 때 실행되는 메서드
//    public void PressButton()
//    {
//        if (!IsPressed)
//        {
//            IsPressed = true;
//            pushEffect?.StartPushEffect();

//            // 타겟 객체가 설정되어 있으면 값을 조절
//            if (buttonControllableTarget != null)
//            {
//                if (changeValueCoroutine != null)
//                    StopCoroutine(changeValueCoroutine);

//                changeValueCoroutine = StartCoroutine(ChangeValueOverTime());
//            }
//        }
//    }

//    private IEnumerator ChangeValueOverTime()
//    {
//        float interval = 1f; // 1초 간격으로 값을 변경
//        while (IsPressed)
//        {
//            if (changeDirection == ValueChangeDirection.Increase)
//                buttonControllableTarget.Increase();
//            else
//                buttonControllableTarget.Decrease();

//            yield return new WaitForSeconds(interval);
//        }
//    }

//    // 버튼 눌림 효과가 끝났을 때 호출되는 메서드
//    private void OnPushComplete()
//    {
//        ReleaseButton(); // 버튼 놓음
//    }

//    // 버튼을 놓는 메서드
//    public void ReleaseButton()
//    {
//        IsPressed = false;

//        if (changeValueCoroutine != null)
//        {
//            StopCoroutine(changeValueCoroutine); // 코루틴 정지
//        }
//    }

//    private void OnDestroy()
//    {
//        if (pushEffect != null)
//        {
//            pushEffect.OnPushComplete -= OnPushComplete; // 이벤트 해제
//        }
//    }
//}

//using UnityEngine;
//using System.Collections;

//public class ValueChangeButton : MonoBehaviour, IInteractable
//{
//    public bool IsPressed { get; private set; }
//    private ThrowerButtonPushEffect pushEffect;

//    // IButtonControllable을 구현한 MonoBehaviour 타입으로 변경
//    [SerializeField] private MonoBehaviour targetObject;  // 조작할 대상 객체, IButtonControllable을 구현한 컴포넌트
//    private IButtonControllable buttonControllableTarget;

//    private Coroutine changeValueCoroutine;     // 값 변경을 위한 코루틴

//    // 방향을 선택할 수 있는 enum 추가
//    public enum ValueChangeDirection
//    {
//        Increase,
//        Decrease
//    }

//    [SerializeField] private ValueChangeDirection changeDirection = ValueChangeDirection.Increase; // 증가/감소 방향 선택

//    private void Awake()
//    {
//        // targetObject가 MonoBehaviour 타입이므로, 이를 IButtonControllable로 캐스팅
//        buttonControllableTarget = targetObject as IButtonControllable;

//        if (buttonControllableTarget == null)
//        {
//            Debug.LogError("targetObject는 IButtonControllable을 구현한 컴포넌트여야 합니다.");
//        }

//        pushEffect = GetComponent<ThrowerButtonPushEffect>();
//        if (pushEffect != null)
//        {
//            pushEffect.OnPushComplete += OnPushComplete;
//        }
//    }

//    // 인터랙션 메서드 - 버튼을 누름
//    public void Interact()
//    {
//        PressButton();
//    }

//    // 버튼을 눌렀을 때 실행되는 메서드
//    public void PressButton()
//    {
//        if (!IsPressed)
//        {
//            IsPressed = true;  // 버튼이 눌렸을 때 상태를 true로 설정
//            pushEffect?.StartPushEffect();

//            // 타겟 객체가 설정되어 있으면 값을 조절
//            if (buttonControllableTarget != null)
//            {
//                // 코루틴이 이미 실행 중이라면 먼저 멈춘 후 다시 시작
//                if (changeValueCoroutine != null)
//                {
//                    StopCoroutine(changeValueCoroutine); // 이전 코루틴 멈춤
//                }

//                changeValueCoroutine = StartCoroutine(ChangeValueOverTime());
//            }
//        }
//        else
//        {
//            // 버튼을 다시 눌렀을 때, 값을 증가시키는 것을 멈춤

//            ReleaseButton();  // 버튼 놓는 메서드 호출
//        }
//    }


//    // 1초 간격으로 값을 증가/감소 시키는 코루틴
//    private IEnumerator ChangeValueOverTime()
//    {
//        float interval = 1f; // 1초 간격으로 값을 변경
//        while (IsPressed)  // 버튼이 눌린 상태에서만 실행
//        {
//            if (changeDirection == ValueChangeDirection.Increase)
//                buttonControllableTarget.Increase();
//            else
//                buttonControllableTarget.Decrease();

//            yield return new WaitForSeconds(interval); // 1초 대기
//        }
//    }

//    // 버튼 눌림 효과가 끝났을 때 호출되는 메서드
//    private void OnPushComplete()
//    {
//        ReleaseButton(); // 버튼 놓음
//    }

//    // 버튼을 놓는 메서드
//    public void ReleaseButton()
//    {
//        IsPressed = false;

//        // 버튼을 놓으면 코루틴을 멈춤
//        if (changeValueCoroutine != null)
//        {
//            StopCoroutine(changeValueCoroutine); // 코루틴 정지
//        }
//    }



//    private void OnDestroy()
//    {
//        if (pushEffect != null)
//        {
//            pushEffect.OnPushComplete -= OnPushComplete; // 이벤트 해제
//        }
//    }
//}

using UnityEngine;
using System.Collections;

public class ValueChangeButton : MonoBehaviour, IInteractable
{
    public bool IsPressed { get; private set; }
    private ThrowerButtonPushEffect pushEffect;

    // IButtonControllable을 구현한 MonoBehaviour 타입으로 변경
    [SerializeField] private MonoBehaviour targetObject;  // 조작할 대상 객체, IButtonControllable을 구현한 컴포넌트
    private IButtonControllable buttonControllableTarget;

    private Coroutine changeValueCoroutine;     // 값 변경을 위한 코루틴
    [Header("호출 시간")]
    float interval = 0.1f;

    // 방향을 선택할 수 있는 enum 추가
    public enum ValueChangeDirection
    {
        Increase,
        Decrease
    }

    [SerializeField] private ValueChangeDirection changeDirection = ValueChangeDirection.Increase; // 증가/감소 방향 선택

    private void Start()
    {
        
    }
    private void Awake()
    {
        // targetObject가 MonoBehaviour 타입이므로, 이를 IButtonControllable로 캐스팅
        buttonControllableTarget = targetObject as IButtonControllable;

        if (buttonControllableTarget == null)
        {
            Debug.LogError("targetObject는 IButtonControllable을 구현한 컴포넌트여야 합니다.");
        }

        pushEffect = GetComponent<ThrowerButtonPushEffect>();
        if (pushEffect != null)
        {
            pushEffect.OnPushComplete += OnPushComplete; // 애니메이션이 완료되면 호출될 이벤트 등록
        }
    }

    // 인터랙션 메서드 - 버튼을 누름
    public void Interact()
    {
        PressButton();
    }



    public void PressButton()
    {
        if (!IsPressed)
        {
            IsPressed = true;  // 버튼이 눌렸을 때 상태를 true로 설정
            pushEffect?.HoldingButton(); // 눌림 효과 시작
            
            
            // 타겟 객체가 설정되어 있으면 값을 조절
            if (buttonControllableTarget != null)
            {
                // 코루틴이 이미 실행 중이라면 먼저 멈춘 후 다시 시작
                if (changeValueCoroutine != null)
                {
                    StopCoroutine(changeValueCoroutine); // 이전 코루틴 멈춤
                }

                changeValueCoroutine = StartCoroutine(ChangeValueOverTime()); // 값을 변경하는 코루틴 시작
            }
            
        }
        else
        {
            // 버튼을 다시 눌렀을 때, 값을 증가시키는 것을 멈춤
            pushEffect.ReleasButton(); // 버튼 놓을 때 효과 멈추기
            ReleaseButton();  // 버튼 놓는 메서드 호출
            IsPressed = false;  // 버튼 놓은 상태로 변경
        }
    }



    // 1초 간격으로 값을 증가/감소 시키는 코루틴
    //private IEnumerator ChangeValueOverTime()
    //{
    //    //float interval = 0.1f; // 1초 간격으로 값을 변경
    //    while (IsPressed)  // 버튼이 눌린 상태에서만 실행
    //    {
    //        if (changeDirection == ValueChangeDirection.Increase)
    //            buttonControllableTarget.Increase();
    //        else
    //            buttonControllableTarget.Decrease();

    //        yield return new WaitForSeconds(interval); // 1초 대기
    //    }
    //}
    private IEnumerator ChangeValueOverTime()
    {
        while (IsPressed)
        {
            // 만약 애니메이션이 끝났다면 자동으로 멈춤
            if (pushEffect != null && !pushEffect._isAnimating)
            {
                pushEffect.ReleasButton();  // 눌림 효과 종료
                ReleaseButton();            // 값 증가/감소 종료
                IsPressed = false;          // 상태 초기화
                yield break;                // 코루틴 종료
            }

            // 값 증가 또는 감소
            if (changeDirection == ValueChangeDirection.Increase)
                buttonControllableTarget.Increase();
            else
                buttonControllableTarget.Decrease();

            yield return new WaitForSeconds(interval);
        }
    }

    // 버튼 눌림 효과가 끝났을 때 호출되는 메서드
    private void OnPushComplete()
    {
        // 애니메이션이 완료되었지만 버튼을 놓지 않음
        // 버튼을 놓지 않고 계속 눌린 상태로 유지할 수 있도록 `ReleaseButton()` 호출을 제거
        // ReleaseButton();  // 버튼 놓기 호출을 제거

        // 이 부분에서 버튼 상태가 변경된 후 계속 눌린 상태로 유지될 수 있습니다.
    }

    // 버튼을 놓는 메서드
    public void ReleaseButton()
    {
        // IsPressed를 false로 설정하지 않고 코루틴을 계속 돌게 유지
        // IsPressed = false; // 이 라인 제거하면 버튼을 놓지 않고 계속 눌려있는 상태 유지

        // 버튼을 놓으면 값을 변경하는 코루틴을 멈춤
        if (changeValueCoroutine != null)
        {
            StopCoroutine(changeValueCoroutine); // 코루틴 정지
        }
    }

    private void OnDestroy()
    {
        if (pushEffect != null)
        {
            pushEffect.OnPushComplete -= OnPushComplete; // 이벤트 해제
        }
    }
}
