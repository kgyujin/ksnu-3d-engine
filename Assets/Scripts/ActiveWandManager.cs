//using UnityEngine;

//public class ActiveWandManager : MonoBehaviour
//{
//    public GameObject[] wands; // 0: Wand1, 1: Wand2, 2: Wand3
//    public RaycastObjectMover raycastMover; // 감지용 스크립트 참조

//    public float[] rayDistances = { 5f, 10f, 15f }; // 각각의 거리 값

//    private int currentIndex = -1;

//    void Start()
//    {
//        SwitchWand(0); // 기본은 Wand1
//    }

//    void Update()
//    {
//        // 키보드 숫자 1, 2, 3 입력 시 해당 인덱스 지팡이 활성화
//        if (Input.GetKeyDown(KeyCode.Alpha1)) SwitchWand(0);
//        if (Input.GetKeyDown(KeyCode.Alpha2)) SwitchWand(1);
//        if (Input.GetKeyDown(KeyCode.Alpha3)) SwitchWand(2);
//    }

//    void SwitchWand(int index)
//    {
//        if (index == currentIndex) return;

//        for (int i = 0; i < wands.Length; i++)
//        {
//            // 선택한 지팡이만 활성화 (나머지는 비활성화)
//            wands[i].SetActive(i == index);
//        }

//        // Raycast 처리에 어떤 지팡이를 사용할지 알려줌
//        raycastMover.rayDistance = rayDistances[index];

//        currentIndex = index;
//    }
//}
