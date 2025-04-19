using UnityEngine;

public class ActiveWandManager : MonoBehaviour
{
    public GameObject[] wands; // 0: Wand1, 1: Wand2, 2: Wand3
    public RaycastObjectMover raycastMover; // 감지용 스크립트 참조

    public float[] rayDistances = { 5f, 10f, 15f }; // 각각의 거리 값

    private int currentIndex = -1;

    void Start()
    {
        SwitchWand(0); // 기본은 Wand1
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) SwitchWand(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SwitchWand(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) SwitchWand(2);
    }

    void SwitchWand(int index)
    {
        if (index == currentIndex) return;

        for (int i = 0; i < wands.Length; i++)
        {
            wands[i].SetActive(i == index); // 해당 지팡이만 보이게
        }

        // Ray 거리 설정
        raycastMover.rayDistance = rayDistances[index];

        currentIndex = index;
    }
}
