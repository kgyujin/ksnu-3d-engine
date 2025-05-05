using System;
using UnityEngine;
public class ItemSelectManager : MonoBehaviour
{
    public GameObject[] Item;
    public float rightOffset = 0.5f;
    private GameObject currentEquippedItem; //현재 장착된 아이템
    RaycastObjectMover raycastObjectMover;
    int key_down = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // 시작할 때 기본 아이템 설정
        ItemSelect(key_down);
        raycastObjectMover = GetComponent<RaycastObjectMover>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            key_down = 0;
            ItemSelect(key_down);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            key_down = 1;
            ItemSelect(key_down);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            key_down = 2;
            ItemSelect(key_down);
        }
    }

    public void WearItem(GameObject gameObject)
    {
        // 현재 장착 중인 아이템이 있으면 비활성화
        if (Item[key_down] != null)
        {
            Item[key_down].SetActive(false);
        }

        // 새 아이템을 현재 선택된 슬롯에 저장
        Item[key_down] = gameObject;

        // 아이템의 물리 속성 처리
        Rigidbody rb = gameObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true; // 물리 영향 비활성화
        }

        // 플레이어의 자식으로 설정 (한 몸이 되도록)
        gameObject.transform.SetParent(transform);

        // 위치 초기화 (로컬 기준)
        gameObject.transform.localPosition = Vector3.right * rightOffset;

        // 방향 초기화 (필요시)
        gameObject.transform.localRotation = Quaternion.identity;

        // 새로운 아이템 활성화
        gameObject.SetActive(true);
        currentEquippedItem = gameObject;
        ItemSelect(key_down);
    }

    private void ItemSelect(int index)
    {
        // 기존 장착 아이템이 있으면 비활성화
        if (currentEquippedItem != null)
        {
            currentEquippedItem.SetActive(false);
        }

        // 선택한 슬롯에 아이템이 없는 경우 처리
        if (Item[index] == null)
        {
            currentEquippedItem = null;

            // 아이템이 없으면 raycastObjectMover.rayDistance를 8f로 설정
            if (raycastObjectMover != null)
            {
                raycastObjectMover.rayDistance = 8f;
            }

            return; // 아이템이 없으면 함수 종료
        }

        // 아이템이 있으면 활성화
        currentEquippedItem = Item[index];
        currentEquippedItem.SetActive(true);
        
        WandItem WandItem = currentEquippedItem.GetComponent<WandItem>();
        
        // 둘 다 존재하는 경우에만 설정
        if (WandItem != null && raycastObjectMover != null)
        {
            raycastObjectMover.rayDistance = WandItem.raycast_distance;
        }
        else
        {
            Debug.LogWarning("WandItem 또는 RaycastObjectMover가 장착된 아이템에 없습니다.");
        }
    }
}